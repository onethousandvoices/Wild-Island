using System;
using UnityEngine;
using UnityEngine.InputSystem;
using WildIsland.Controllers;
using WildIsland.Data;
using WildIsland.Views;
using Zenject;
using PlayerInput = Core.PlayerInput;

namespace WildIsland.Processors
{
    public enum PlayerInputState : byte
    {
        Idle,
        Run,
        Sprint,
        Jump
    }

    public class PlayerInputProcessor : BaseProcessor, IInitializable, IFixedPlayerProcessor, IPlayerSpeed, IPlayerInputState, ILateTickable, IDisposable
    {
        [Inject] private Camera _mainCamera;
        [Inject] private PlayerView _view;
        [Inject] private IPlayerStatSetter _statSetter;
        [Inject] private IGetPlayerStats _playerStats;
        [Inject] private IPlayerInventory _inventory;
        [Inject] private IGetCheats _cheats;

        private PlayerData _stats;
        private PlayerInput _playerInput;
        private InputMap _inputMap;
        private Animator _animator;
        private CapsuleCollider _capsuleCollider;

        private bool _isLockCameraPosition;
        private bool _isGrounded = true;

        private float _moveSpeed;
        private float _sprintSpeed;
        private float _cameraAngleOverride;
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        private float _relativeSpeed;
        private float _animationBlend;
        private float _targetRotation;
        private float _rotationVelocity;
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private float _staminaJumpCost;
        private float _staminaSprintCost;

        private const float _inAirVelocityReduction = 0.95f;
        private const float _inputMagnitude = 1f;
        private const float _speedUpChangeRate = 1.1f;
        private const float _slowDownChangeRate = 7f;
        private const float _rotationSmoothTime = 0.12f;
        private const float _jumpTimeout = 0.1f;
        private const float _fallTimeout = 0.15f;
        private const float _groundedOffset = 0.05f;
        private const float _groundCheckSphereRadius = 0.15f;
        private const float _topClamp = 70f;
        private const float _bottomClamp = -30f;
        private const float _threshold = 0.01f;
        private const float _deltaTimeMultiplier = 1f;

        private static readonly int _animIDSpeed = Animator.StringToHash("Speed");
        private static readonly int _animIDGrounded = Animator.StringToHash("Grounded");
        private static readonly int _animIDJump = Animator.StringToHash("Jump");
        private static readonly int _animIDFreeFall = Animator.StringToHash("FreeFall");

        private bool _jumpPossible => _staminaJumpCost < _stats.Stamina.Value;
        private bool _sprintPossible => _stats.Stamina.Value - _staminaJumpCost * Time.deltaTime > 0;

        public PlayerInputState State { get; private set; }
        public float CurrentSpeed { get; private set; }

        public void Initialize()
        {
            State = PlayerInputState.Idle;
            _stats = _playerStats.Stats;

            _jumpTimeoutDelta = _jumpTimeout;
            _fallTimeoutDelta = _fallTimeout;

            _staminaJumpCost = _view.StaminaJumpCost / _playerStats.Stats.Stamina.Default * 100;
            _staminaSprintCost = _view.StaminaSprintCost / _playerStats.Stats.Stamina.Default * 100;

            _moveSpeed = _stats.RegularSpeed.Value;
            _sprintSpeed = _stats.SprintSpeed.Value;

            _capsuleCollider = _view.GetComponent<CapsuleCollider>();
            _animator = _view.GetComponent<Animator>();
            _cinemachineTargetYaw = _view.CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            BindInputs();
        }

        public void Dispose()
            => _inputMap.Dispose();

        public void Tick()
        {
            if (!Enabled)
                return;

            ReadInput();
        }

        public void FixedTick()
        {
            if (!Enabled)
                return;

            Move();
            JumpAndGravity();
            GroundedCheck();
        }

        public void LateTick()
            => CameraRotation();

        private void BindInputs()
        {
            _playerInput = new PlayerInput();
            _inputMap = new InputMap();
            _inputMap.Enable();
            
            _inputMap.Player.Jump.performed += OnJumpPerformed;
            _inputMap.Player.Inventory.performed += _inventory.ShowInventory;
            
            _inputMap.Player.CHEAT_Time.started += _cheats.CHEAT_TimeSpeedUp;
            _inputMap.Player.CHEAT_Damage.started += _cheats.CHEAT_Damage;
            _inputMap.Player.CHEAT_FrameRate.started += _cheats.CHEAT_FrameRateChange;
            _inputMap.Player.CHEAT_PeriodicEffect.started += _cheats.CHEAT_PeriodicEffectApply;
            _inputMap.Player.CHEAT_TemporaryEffect.started += _cheats.CHEAT_TemporaryEffectApply;
        }
        
        private void ReadInput()
        {
            _playerInput.SetLook(_inputMap.Player.Look.ReadValue<Vector2>());
            _playerInput.SetMove(_inputMap.Player.Move.ReadValue<Vector2>());
            CheckSprint();
        }

        private void OnJumpPerformed(InputAction.CallbackContext obj)
        {
            if (!_jumpPossible || !_isGrounded || _jumpTimeoutDelta > 0f)
                return;
            _view.Rb.velocity = new Vector3(_view.Rb.velocity.x, _view.JumpHeight, _view.Rb.velocity.z);
            _playerInput.SetJump(obj.ReadValueAsButton());
            _statSetter.SetStat(_stats.Stamina, -_staminaJumpCost, true);
        }

        private void CheckSprint()
        {
            bool isSprinting = _inputMap.Player.Sprint.IsPressed();

            if (!isSprinting)
            {
                _playerInput.SetSprint(false);
                return;
            }

            if (!_sprintPossible)
            {
                _playerInput.SetSprint(false);
                return;
            }

            _playerInput.SetSprint(true);
            _statSetter.SetStat(_stats.Stamina, -_staminaSprintCost * Time.deltaTime, true);
        }

        private void JumpAndGravity()
        {
            if (_isGrounded)
            {
                _fallTimeoutDelta = _fallTimeout;

                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);

                if (_playerInput.Jump && _jumpTimeoutDelta <= 0f)
                    _animator.SetBool(_animIDJump, true);

                if (_jumpTimeoutDelta >= 0.0f)
                    _jumpTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _jumpTimeoutDelta = _jumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                    _fallTimeoutDelta -= Time.deltaTime;

                _animator.SetBool(_animIDFreeFall, true);
                _playerInput.ResetJump();
            }
        }

        private void GroundedCheck()
        {
            Vector3 playerPos = _view.transform.position;
            Vector3 spherePosition = new Vector3(playerPos.x, playerPos.y - _groundedOffset, playerPos.z);
            _isGrounded = Physics.CheckSphere(spherePosition, _groundCheckSphereRadius, _view.GroundLayers, QueryTriggerInteraction.Ignore);

            if (_isGrounded && _playerInput.Move.sqrMagnitude == 0)
                _capsuleCollider.material = _view.FrictionMaterial;
            else
                _capsuleCollider.material = _view.SlipperyMaterial;

            State = _isGrounded ? PlayerInputState.Idle : PlayerInputState.Jump;

            _animator.SetBool(_animIDGrounded, _isGrounded);
        }

        private void Move()
        {
            float targetSpeed;
            PlayerInputState pendingInputState;

            if (_playerInput.Sprint)
            {
                targetSpeed = _sprintSpeed;
                pendingInputState = PlayerInputState.Sprint;
            }
            else
            {
                targetSpeed = _moveSpeed;
                pendingInputState = PlayerInputState.Run;
            }

            if (_playerInput.Move == Vector2.zero)
            {
                targetSpeed = 0.0f;
                pendingInputState = PlayerInputState.Idle;
            }

            if (_isGrounded)
                State = pendingInputState;

            float rbSpeed = new Vector3(_view.Rb.velocity.x, 0f, _view.Rb.velocity.z).magnitude;
            float modifier = _playerInput.Move.sqrMagnitude > 0 ? _speedUpChangeRate : _slowDownChangeRate;

            CurrentSpeed = Mathf.Lerp(rbSpeed, targetSpeed * _inputMagnitude,
                Time.fixedDeltaTime * modifier);

            CurrentSpeed = Mathf.Round(CurrentSpeed * 1000f) / 1000f;

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.fixedDeltaTime * modifier * 5f);
            if (_animationBlend < 0.01f)
                _animationBlend = 0f;

            Vector3 inputDirection = new Vector3(_playerInput.Move.x, 0.0f, _playerInput.Move.y).normalized;
            
            if (_playerInput.Move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;

                float rotation = Mathf.SmoothDampAngle(_view.transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    _rotationSmoothTime);

                _view.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            if (!_isGrounded)
                CurrentSpeed *= _inAirVelocityReduction;

            Vector3 currentVelocity = _view.Rb.velocity;
            targetDirection *= CurrentSpeed;

            Vector3 velocityChange = targetDirection - currentVelocity;
            velocityChange = new Vector3(velocityChange.x, 0f, velocityChange.z);
            velocityChange = AdjustSlopeVelocity(velocityChange);
            velocityChange = Vector3.ClampMagnitude(velocityChange, CurrentSpeed);

            _view.Rb.AddForce(velocityChange, ForceMode.VelocityChange);
            
            _animator.SetFloat(_animIDSpeed, _animationBlend);
        }

        private Vector3 AdjustSlopeVelocity(Vector3 velocity)
        {
            Ray ray = new Ray(_view.transform.position, Vector3.down);
            if (!Physics.Raycast(ray, out RaycastHit slopeHit, 0.2f))
                return velocity;
            Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, slopeHit.normal);
            Vector3 adjustedVelocity = slopeRotation * velocity;

            return adjustedVelocity.y < 0 ? adjustedVelocity : velocity;
        }

        private void CameraRotation()
        {
            if (_playerInput.Look.sqrMagnitude >= _threshold && !_isLockCameraPosition)
            {
                _cinemachineTargetYaw += _playerInput.Look.x * _deltaTimeMultiplier;
                _cinemachineTargetPitch += _playerInput.Look.y * _deltaTimeMultiplier;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _bottomClamp, _topClamp);

            _view.CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + _cameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f)
                lfAngle += 360f;
            if (lfAngle > 360f)
                lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }

    public interface IPlayerSpeed
    {
        public float CurrentSpeed { get; }
    }

    public interface IPlayerInputState
    {
        public PlayerInputState State { get; }
    }
}
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using WildIsland.Controllers;
using WildIsland.Data;
using WildIsland.Views;
using Zenject;

namespace WildIsland.Processors
{
    public enum MoveState : byte
    {
        Idle,
        Run,
        Sprint,
        Jump
    }

    [Flags]
    public enum InputState : byte
    {
        None = 1,
        BlockInventory = 2,
        BlockJump = 4,
        BlockCamera = 8,
        BlockSprint = 16,
        BlockMove = 32,
        ShowCursor = 64
    }

    public class PlayerProcessor : BaseProcessor, IInitializable, IFixedPlayerProcessor, IPlayerState, ILateTickable, IDisposable
    {
        [Inject] private Camera _mainCamera;
        [Inject] private PlayerView _view;
        [Inject] private IPlayerStatSetter _statSetter;
        [Inject] private IGetPlayerStats _playerStats;
        [Inject] private IPlayerInventory _inventory;
        [Inject] private IConsoleHandler _consoleHandler;

        private PlayerData _stats;
        private InputMap _inputMap;
        private Animator _animator;
        private CapsuleCollider _capsuleCollider;
        private Vector2 _speedBlend;
        private Vector2 _sprintBlend;
        private Vector2 _look;
        private Vector2 _move;
        private Vector2 _lastMove;

        private bool _sprint;
        private bool _jump;
        private bool _isLockCameraPosition;
        private bool _isGrounded = true;

        private float _moveSpeed;
        private float _sprintSpeed;
        private float _cameraAngleOverride;
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        private float _relativeSpeed;
        private float _targetRotation;
        private float _rotationVelocity;
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private float _staminaJumpCost;
        private float _staminaSprintCost;

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

        private static readonly int _animSpeedX = Animator.StringToHash("SpeedX");
        private static readonly int _animSpeedY = Animator.StringToHash("SpeedY");
        private static readonly int _animGrounded = Animator.StringToHash("Grounded");
        private static readonly int _animJump = Animator.StringToHash("Jump");
        private static readonly int _animFreeFall = Animator.StringToHash("FreeFall");

        private bool _jumpPossible => _staminaJumpCost < _stats.Stamina.Value;
        private bool _sprintPossible => _stats.Stamina.Value - _staminaJumpCost * Time.deltaTime > 0;

        public MoveState MoveState { get; private set; }
        public InputState InputState { get; private set; }

        public float CurrentSpeed { get; private set; }

        public void Initialize()
        {
            MoveState = MoveState.Idle;
            _stats = _playerStats.Stats;

            _jumpTimeoutDelta = _jumpTimeout;
            _fallTimeoutDelta = _fallTimeout;

            _staminaJumpCost = _view.StaminaJumpCost / _playerStats.Stats.Stamina.Default * 100;
            _staminaSprintCost = _view.StaminaSprintCost / _playerStats.Stats.Stamina.Default * 100;

            _moveSpeed = _stats.RegularSpeed.Value;
            _sprintSpeed = _stats.SprintSpeed.Value;

            _capsuleCollider = _view.GetComponent<CapsuleCollider>();
            _animator = _view.Animator;
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
            CheckCursor();
        }

        public void FixedTick()
        {
            if (!Enabled)
                return;

            Movement();
            JumpAndGravity();
            GroundedCheck();
        }

        public void LateTick()
            => CameraRotation();

        private void BindInputs()
        {
            _inputMap = new InputMap();
            _inputMap.Enable();

            _inputMap.Player.Jump.performed += OnJumpPerformed;
            _inputMap.Player.Inventory.performed += _ => _inventory.ShowInventory();

            _inputMap.Player.Console.performed += _ => _consoleHandler.ShowConsole();
            _inputMap.Player.ReturnButton.performed += _ => _consoleHandler.OnReturn();
            _inputMap.Player.ArrowUp.performed += _ => _consoleHandler.OnUpArrow();
        }

        private void CheckCursor()
        {
            if (InputState.HasFlag(InputState.ShowCursor))
            {
                Cursor.lockState = CursorLockMode.Confined;
                return;
            }

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void ReadInput()
        {
            _look = _inputMap.Player.Look.ReadValue<Vector2>();
            Vector2 move = _inputMap.Player.Move.ReadValue<Vector2>();
            if (_sprint)
                move = Vector2.up;
            _move = move;
            CheckSprint();
        }

        private void OnJumpPerformed(InputAction.CallbackContext obj)
        {
            if (!_jumpPossible || !_isGrounded || _jumpTimeoutDelta > 0f || InputState.HasFlag(InputState.BlockJump))
                return;
            _lastMove = _inputMap.Player.Move.ReadValue<Vector2>();
            _view.Rb.velocity = new Vector3(_view.Rb.velocity.x, _view.JumpHeight, _view.Rb.velocity.z);
            _jump = true;
            _statSetter.SetStat(_stats.Stamina, -_staminaJumpCost, true);
        }

        private void CheckSprint()
        {
            bool isSprinting = _inputMap.Player.Sprint.IsPressed();

            if (!isSprinting)
            {
                _sprint = false;
                return;
            }

            if (!_sprintPossible || _move != Vector2.up)
            {
                _sprint = false;
                return;
            }

            _sprint = true;
            _statSetter.SetStat(_stats.Stamina, -_staminaSprintCost * Time.deltaTime, true);
        }

        private void JumpAndGravity()
        {
            if (_isGrounded)
            {
                _fallTimeoutDelta = _fallTimeout;

                _animator.SetBool(_animJump, false);
                _animator.SetBool(_animFreeFall, false);

                if (_jump && _jumpTimeoutDelta <= 0f)
                    _animator.SetBool(_animJump, true);

                if (_jumpTimeoutDelta >= 0f)
                    _jumpTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _jumpTimeoutDelta = _jumpTimeout;

                if (_fallTimeoutDelta >= 0f)
                    _fallTimeoutDelta -= Time.deltaTime;

                _animator.SetBool(_animFreeFall, true);
                _jump = false;
            }
        }

        private void GroundedCheck()
        {
            Vector3 playerPos = _view.transform.position;
            Vector3 spherePosition = new Vector3(playerPos.x, playerPos.y - _groundedOffset, playerPos.z);
            _isGrounded = Physics.CheckSphere(spherePosition, _groundCheckSphereRadius, _view.GroundLayers, QueryTriggerInteraction.Ignore);

            switch (_isGrounded)
            {
                case true:
                    MoveState = MoveState.Idle;
                    _capsuleCollider.material = _move.sqrMagnitude == 0
                                                    ? _view.FrictionMaterial
                                                    : _view.SlipperyMaterial;

                    if (_fallTimeoutDelta <= 0f)
                        _lastMove = Vector2.zero;
                    break;
                case false:
                    MoveState = MoveState.Jump;
                    _capsuleCollider.material = _view.SlipperyMaterial;
                    break;
            }

            _animator.SetBool(_animGrounded, _isGrounded);
        }

        private void Movement()
        {
            if (InputState.HasFlag(InputState.BlockMove))
                return;
            
            float targetSpeed;
            MoveState pendingState;

            if (_sprint)
            {
                targetSpeed = _sprintSpeed;
                pendingState = MoveState.Sprint;
            }
            else
            {
                targetSpeed = _moveSpeed;
                pendingState = MoveState.Run;
            }

            if (_move == Vector2.zero)
            {
                targetSpeed = 0.0f;
                pendingState = MoveState.Idle;
            }

            if (_isGrounded)
                MoveState = pendingState;

            float rbSpeed = new Vector3(_view.Rb.velocity.x, 0f, _view.Rb.velocity.z).magnitude;
            float modifier = _move.sqrMagnitude > 0 ? _speedUpChangeRate : _slowDownChangeRate;

            CurrentSpeed = Mathf.Lerp(rbSpeed, targetSpeed * _inputMagnitude,
                Time.fixedDeltaTime * modifier);

            CurrentSpeed = Mathf.Round(CurrentSpeed * 1000f) / 1000f;

            Vector3 inputDirection = _isGrounded
                                         ? new Vector3(_move.x, 0.0f, _move.y).normalized
                                         : new Vector3(_lastMove.x, 0.0f, _lastMove.y).normalized;

            if (_move != Vector2.zero && _isGrounded)
            {
                _targetRotation = _mainCamera.transform.eulerAngles.y;

                float rotation = Mathf.SmoothDampAngle(_view.transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    _rotationSmoothTime);

                _view.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            float horizontalVelocity = new Vector2(_move.x, 0f).magnitude;

            if (horizontalVelocity > 0)
                CurrentSpeed *= _view.HorizontalVelocityReduction;

            if (_move.y < 0)
                CurrentSpeed *= _view.BackwardsVelocityReduction;

            if (!_isGrounded)
                CurrentSpeed *= _view.InAirVelocityReduction;

            Vector3 targetDirection = Quaternion.Euler(0f, _targetRotation, 0f) * inputDirection;

            Vector3 currentVelocity = _view.Rb.velocity;
            targetDirection *= CurrentSpeed;

            Vector3 velocityChange = targetDirection - currentVelocity;
            velocityChange = new Vector3(velocityChange.x, 0f, velocityChange.z);
            velocityChange = AdjustSlopeVelocity(velocityChange);
            velocityChange = Vector3.ClampMagnitude(velocityChange, CurrentSpeed);

            _view.Rb.AddForce(velocityChange, ForceMode.VelocityChange);

            _sprintBlend = new Vector2(0f, _sprint ? 1 : 0);
            _speedBlend = Vector3.Lerp(_speedBlend, _move + _sprintBlend, Time.fixedDeltaTime * modifier * 5f);

            _animator.SetFloat(_animSpeedX, _speedBlend.x);
            _animator.SetFloat(_animSpeedY, _speedBlend.y);
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
            if (InputState.HasFlag(InputState.BlockCamera))
                return;

            if (_look.sqrMagnitude >= _threshold && !_isLockCameraPosition)
            {
                _cinemachineTargetYaw += _look.x * _deltaTimeMultiplier;
                _cinemachineTargetPitch += _look.y * _deltaTimeMultiplier;
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

        public void AddState(params InputState[] newState)
        {
            foreach (InputState state in newState)
                InputState |= state;
        }

        public void RemoveState(params InputState[] removeState)
        {
            foreach (InputState state in removeState)
                InputState &= ~state;
        }

        public void AddAllExcept(InputState state)
        {
            Array states = Enum.GetValues(typeof(InputState));

            foreach (InputState inputState in states)
            {
                if (inputState == state)
                    continue;
                InputState |= inputState;
            }
        }

        public void RemoveAllExcept(InputState state)
        {
            Array states = Enum.GetValues(typeof(InputState));

            foreach (InputState inputState in states)
            {
                if (inputState == state)
                    continue;
                InputState &= ~inputState;
            }
        }
    }

    public interface IPlayerState
    {
        public MoveState MoveState { get; }
        public InputState InputState { get; }

        public float CurrentSpeed { get; }

        public void AddState(params InputState[] newStates);
        public void RemoveState(params InputState[] removeStates);
        public void AddAllExcept(InputState state);
        public void RemoveAllExcept(InputState state);
    }
}
﻿using System;
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
    
    public class PlayerInputProcessor : PlayerProcessor, IPlayerSpeed, IPlayerInputState, ILateTickable, IDisposable
    {
        [Inject] private Camera _mainCamera;
        [Inject] private PlayerView _view;
        [Inject] private IPlayerStatSetter _statSetter;
        [Inject] private IGetPlayerStats _playerStats;
        [Inject] private IGetCheats _cheats;

        private PlayerData _stats;
        private PlayerInput _playerInput;
        private InputMap _inputMap;
        private Animator _animator;

        private bool _hasAnimator;
        private bool _isLockCameraPosition;
        private bool _isGrounded = true;
        private bool _isTimeSpeedUp;
        private bool _isFrameRate60;

        private float _moveSpeed;
        private float _sprintSpeed;
        private float _cameraAngleOverride;
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        private float _relativeSpeed;
        private float _animationBlend;
        private float _targetRotation;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private float _staminaJumpCost;
        private float _staminaSprintCost;

        private const float _inAirVelocityReduction = 0.98f;
        private const float _inputMagnitude = 1f;
        private const float _speedUpChangeRate = 1.5f;
        private const float _slowDownChangeRate = 10f;
        private const float _rotationSmoothTime = 0.12f;
        private const float _jumpHeight = 1.7f;
        private const float _gravity = -15f;
        private const float _jumpTimeout = 0.1f;
        private const float _fallTimeout = 0.15f;
        private const float _groundedOffset = -0.14f;
        private const float _groundedRadius = 0.28f;
        private const float _topClamp = 70f;
        private const float _bottomClamp = -30f;
        private const float _terminalVelocity = 53f;
        private const float _threshold = 0.01f;
        private const float _deltaTimeMultiplier = 1f;

        private static readonly int _animIDSpeed = Animator.StringToHash("Speed");
        private static readonly int _animIDGrounded = Animator.StringToHash("Grounded");
        private static readonly int _animIDJump = Animator.StringToHash("Jump");
        private static readonly int _animIDFreeFall = Animator.StringToHash("FreeFall");
        private static readonly int _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

        private bool _jumpPossible => _staminaJumpCost < _stats.Stamina.Value;
        private bool _sprintPossible => _stats.Stamina.Value - _staminaJumpCost * Time.deltaTime > 0;

        public PlayerInputState State { get; private set; }
        public float CurrentSpeed { get; private set; }

        public override void Initialize()
        {
            State = PlayerInputState.Idle;
            _stats = _playerStats.Stats;

            _jumpTimeoutDelta = _jumpTimeout;
            _fallTimeoutDelta = _fallTimeout;

            _staminaJumpCost = _view.StaminaJumpCost / _playerStats.Stats.Stamina.Default * 100;
            _staminaSprintCost = _view.StaminaSprintCost / _playerStats.Stats.Stamina.Default * 100;

            _moveSpeed = _stats.RegularSpeed.Value;
            _sprintSpeed = _stats.SprintSpeed.Value;

            _hasAnimator = _view.TryGetComponent(out _animator);
            _cinemachineTargetYaw = _view.CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _playerInput = new PlayerInput();
            _inputMap = new InputMap();
            _inputMap.Enable();

            _inputMap.Player.Jump.performed += OnJumpPerformed;
            _inputMap.Player.CHEAT_Time.started += _cheats.CHEAT_TimeSpeedUp;
            _inputMap.Player.CHEAT_Damage.started += _cheats.CHEAT_Damage;
            _inputMap.Player.CHEAT_FrameRate.started += _cheats.CHEAT_FrameRateChange;
            _inputMap.Player.CHEAT_PeriodicEffect.started += _cheats.CHEAT_PeriodicEffectApply;
            _inputMap.Player.CHEAT_TemporaryEffect.started += _cheats.CHEAT_TemporaryEffectApply;
        }

        public void Dispose()
            => _inputMap.Dispose();

        public override void Tick()
        {
            ReadInput();
            GroundedCheck();
            Move();
            JumpAndGravity();
        }

        public void LateTick()
            => CameraRotation();

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

                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                if (_verticalVelocity < 0.0f)
                    _verticalVelocity = -2f;

                if (_playerInput.Jump && _jumpTimeoutDelta <= 0f)
                {
                    _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);

                    if (_hasAnimator)
                        _animator.SetBool(_animIDJump, true);
                }

                if (_jumpTimeoutDelta >= 0.0f)
                    _jumpTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _jumpTimeoutDelta = _jumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                    _fallTimeoutDelta -= Time.deltaTime;
                else if (_hasAnimator)
                    _animator.SetBool(_animIDFreeFall, true);
                _playerInput.ResetJump();
            }

            if (_verticalVelocity < _terminalVelocity)
                _verticalVelocity += _gravity * Time.deltaTime;
        }

        private void GroundedCheck()
        {
            Vector3 playerPos = _view.transform.position;
            Vector3 spherePosition = new Vector3(playerPos.x, playerPos.y - _groundedOffset,
                playerPos.z);
            _isGrounded = Physics.CheckSphere(spherePosition, _groundedRadius, _view.GroundLayers,
                QueryTriggerInteraction.Ignore);

            State = _isGrounded ? PlayerInputState.Idle : PlayerInputState.Jump;

            if (_hasAnimator)
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

            float currentHorizontalSpeed = new Vector3(_view.CharacterController.velocity.x, 0.0f, _view.CharacterController.velocity.z).magnitude;
            float multiplier = currentHorizontalSpeed < targetSpeed ? _speedUpChangeRate : _slowDownChangeRate;

            if (currentHorizontalSpeed < targetSpeed || currentHorizontalSpeed > targetSpeed)
            {
                CurrentSpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * _inputMagnitude,
                    Time.deltaTime * multiplier);

                CurrentSpeed = Mathf.Round(CurrentSpeed * 1000f) / 1000f;
            }
            else
                CurrentSpeed = targetSpeed;

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * multiplier * 5f);
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

            _view.CharacterController.Move(targetDirection.normalized * (CurrentSpeed * Time.deltaTime) +
                                           new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            if (!_hasAnimator)
                return;
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, _inputMagnitude);
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
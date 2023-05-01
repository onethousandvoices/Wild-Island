using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using WildIsland.Data;
using WildIsland.Views;
using WildIsland.Views.UI;
using Zenject;
using PlayerInput = Core.PlayerInput;
using Random = UnityEngine.Random;

namespace WildIsland.Controllers
{
    public enum PlayerInputState : byte
    {
        Idle,
        Run,
        Sprint,
        Jump
    }

    public class PlayerController : IInitializable, ITickable, ILateTickable, IDisposable, IGDConsumer
    {
        [Inject] private Camera _mainCamera;
        [Inject] private PlayerView _view;
        [Inject] private PlayerViewStatsHolder _viewStatsHolder;
#region Fields
        private DbValue<PlayerData> _data;
        private PlayerData _stats => _data.Value;
        private PlayerData _statsContainer;
        private PlayerInputState _playerInputState;
        private PlayerInput _playerInput;
        private InputMap _inputMap;
        private Animator _animator;
        private CharacterController _controller;
        private Dictionary<Type, PlayerStat> _statsByType;

        private bool _hasAnimator;
        private bool _isLockCameraPosition;
        private bool _isGrounded = true;
        private bool _isTimeSpeedUp;

        private float _moveSpeed;
        private float _sprintSpeed;
        private float _cameraAngleOverride;
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        private float _speed;
        private float _relativeSpeed;
        private float _animationBlend;
        private float _targetRotation;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private float _staminaJumpCost;
        private float _staminaSprintCost;

        private const float _inputMagnitude = 1f;
        private const float _speedUpChangeRate = 1.5f;
        private const float _slowDownChangeRate = 10f;
        private const float _rotationSmoothTime = 0.12f;
        private const float _footstepAudioVolume = 0.5f;
        private const float _jumpHeight = 1.7f;
        private const float _gravity = -15f;
        private const float _jumpTimeout = 0.5f;
        private const float _fallTimeout = 0.15f;
        private const float _gGroundedOffset = -0.14f;
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
#endregion
        private bool _jumpPossible => _staminaJumpCost < _stats.Stamina.Value;
        private bool _sprintPossible => _stats.Stamina.Value - _staminaJumpCost * Time.deltaTime > 0;

        public Type ContainerType => typeof(PlayerDataContainer);

        public void AcquireGameData(IPartialGameDataContainer container)
            => _statsContainer = ((PlayerDataContainer)container).Default;

        public void Initialize()
        {
            SetData();
            SetStatViews();
            InitInputActions();

            _cinemachineTargetYaw = _view.CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = _view.TryGetComponent(out _animator);
            _controller = _view.GetComponent<CharacterController>();
            _playerInput = new PlayerInput();

            _view.SetOnLandCallback(Land);
            _view.SetOnFootStepCallback(Footstep);
        }

        public void Tick()
        {
            ReadInput();
            UpdateStats();

            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        public void LateTick()
            => CameraRotation();

        public void Dispose()
        {
            _inputMap.Dispose();
            //todo remove container
            _data.Save(_statsContainer);
        }

#region InitActions
        private void SetData()
        {
            _playerInputState = PlayerInputState.Idle;
            _data = new DbValue<PlayerData>("PlayerData", _statsContainer);

            _jumpTimeoutDelta = _jumpTimeout;
            _fallTimeoutDelta = _fallTimeout;

            _staminaJumpCost = _view.StaminaJumpCost / _statsContainer.Stamina.Value * 100;
            _staminaSprintCost = _view.StaminaSprintCost / _statsContainer.Stamina.Value * 100;

            _moveSpeed = _data.Value.RegularSpeed.Value;
            _sprintSpeed = _data.Value.SprintSpeed.Value;

            _statsByType = new Dictionary<Type, PlayerStat>
            {
                { typeof(PlayerHeadHealth), _stats.HeadHealth },
                { typeof(PlayerBodyHealth), _stats.BodyHealth },
                { typeof(PlayerLeftArmHealth), _stats.LeftArmHealth },
                { typeof(PlayerRightArmHealth), _stats.RightArmHealth },
                { typeof(PlayerLeftLegHealth), _stats.LeftLegHealth },
                { typeof(PlayerRightLegHealth), _stats.RightLegHealth },
                { typeof(PlayerHealthRegen), _stats.HealthRegen },
                { typeof(PlayerStamina), _stats.Stamina },
                { typeof(PlayerStaminaRegen), _stats.StaminaRegen },
                { typeof(PlayerHunger), _stats.Hunger },
                { typeof(PlayerHungerDecrease), _stats.HungerDecrease },
                { typeof(PlayerThirst), _stats.Thirst },
                { typeof(PlayerThirstDecrease), _stats.ThirstDecrease },
                { typeof(PlayerFatigue), _stats.Fatigue },
                { typeof(PlayerFatigueDecrease), _stats.FatigueDecrease },
                { typeof(PlayerRegularSpeed), _stats.RegularSpeed },
                { typeof(PlayerSprintSpeed), _stats.SprintSpeed },
                { typeof(PlayerTemperature), _stats.Temperature },
                { typeof(PlayerHealthRegenHungerStage1), _stats.HealthRegenHungerStage1 },
                { typeof(PlayerHealthRegenHungerStage2), _stats.HealthRegenHungerStage2 },
                { typeof(PlayerHealthRegenHungerStage3), _stats.HealthRegenHungerStage3 },
                { typeof(PlayerHealthRegenHungerStage4), _stats.HealthRegenHungerStage4 },
                { typeof(PlayerHealthRegenThirstStage1), _stats.HealthRegenThirstStage1 },
                { typeof(PlayerHealthRegenThirstStage2), _stats.HealthRegenThirstStage2 },
                { typeof(PlayerHealthRegenThirstStage3), _stats.HealthRegenThirstStage3 },
                { typeof(PlayerHealthRegenThirstStage4), _stats.HealthRegenThirstStage4 },
            };
        }

        private void SetStatViews()
        {
            _viewStatsHolder.PlayerBodyStatView.SetRefs(_stats.BodyHealth, _statsContainer.BodyHealth);
            _viewStatsHolder.PlayerHeadStatView.SetRefs(_stats.HeadHealth, _statsContainer.HeadHealth);
            _viewStatsHolder.PlayerLeftArmStatView.SetRefs(_stats.LeftLegHealth, _statsContainer.LeftLegHealth);
            _viewStatsHolder.PlayerRightArmStatView.SetRefs(_stats.RightArmHealth, _statsContainer.RightArmHealth);
            _viewStatsHolder.PlayerLeftLegStatView.SetRefs(_stats.LeftLegHealth, _statsContainer.LeftLegHealth);
            _viewStatsHolder.PlayerRightLegStatView.SetRefs(_stats.RightLegHealth, _statsContainer.RightLegHealth);
            _viewStatsHolder.PlayerStaminaStatView.SetRefs(_stats.Stamina, _statsContainer.Stamina);
            _viewStatsHolder.PlayerHungerStatView.SetRefs(_stats.Hunger, _statsContainer.Hunger);
            _viewStatsHolder.PlayerThirstStatView.SetRefs(_stats.Thirst, _statsContainer.Thirst);
            _viewStatsHolder.PlayerFatigueStatView.SetRefs(_stats.Fatigue, _statsContainer.Fatigue);
            _viewStatsHolder.PlayerTemperatureStatView.SetRefs(_stats.Temperature, _statsContainer.Temperature);
        }

        private void InitInputActions()
        {
            _inputMap = new InputMap();
            _inputMap.Enable();

            _inputMap.Player.Jump.performed += OnJumpPerformed;
            _inputMap.Player.CHEAT_Time.started += CHEAT_TimeSpeedUp;
            _inputMap.Player.CHEAT_Damage.started += CHEAT_Damage;
        }
#endregion
#region Data
        private void UpdateStats()
        {
            _relativeSpeed = _speed / _stats.SprintSpeed.Value;

            ProcessHealth();
            ProcessStamina();
            ProcessHunger();
            ProcessFatigue();
            ProcessThirst();
        }

        private void ProcessHealth()
        {
            float hungerMod = 0f;
            float thirstMod = 0f;
            float currentHunger = _stats.Hunger.Value;
            float currentThirst = _stats.Thirst.Value;

            if (currentHunger <= _view.HungerRegenStage1Range.y && currentHunger >= _view.HungerRegenStage1Range.x)
                hungerMod = _statsContainer.HealthRegenHungerStage1.Value;
            else if (currentHunger <= _view.HungerRegenStage2Range.y && currentHunger >= _view.HungerRegenStage2Range.x)
                hungerMod = _statsContainer.HealthRegenHungerStage2.Value;
            else if (currentHunger <= _view.HungerRegenStage3Range.y && currentHunger >= _view.HungerRegenStage3Range.x)
                hungerMod = _statsContainer.HealthRegenHungerStage3.Value;
            else if (currentHunger <= _view.HungerRegenStage4Range.y)
                hungerMod = _statsContainer.HealthRegenHungerStage4.Value;

            if (currentThirst <= _view.ThirstRegenStage1Range.y && currentThirst >= _view.ThirstRegenStage1Range.x)
                thirstMod = _statsContainer.HealthRegenThirstStage1.Value;
            else if (currentThirst <= _view.ThirstRegenStage2Range.y && currentThirst >= _view.ThirstRegenStage2Range.x)
                thirstMod = _statsContainer.HealthRegenThirstStage2.Value;
            else if (currentThirst <= _view.ThirstRegenStage3Range.y && currentThirst >= _view.ThirstRegenStage3Range.x)
                thirstMod = _statsContainer.HealthRegenThirstStage3.Value;
            else if (currentThirst <= _view.ThirstRegenStage4Range.y)
                thirstMod = _statsContainer.HealthRegenThirstStage4.Value;

            float currentRegen = _stats.HealthRegen.Value + hungerMod + thirstMod;
            SetAllHealths(currentRegen * Time.deltaTime);
        }

        private void SetAllHealths(float value = 0f, bool isRandomizing = false)
        {
            bool decreasing = value < 0;
            
            if (_stats.HeadHealth.Value < _statsContainer.HeadHealth.Value || decreasing || isRandomizing)
                SetStat(_viewStatsHolder.PlayerHeadStatView, _stats.HeadHealth.Value + value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
            if (_stats.BodyHealth.Value < _statsContainer.BodyHealth.Value || decreasing || isRandomizing)
                SetStat(_viewStatsHolder.PlayerBodyStatView, _stats.BodyHealth.Value + value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
            if (_stats.LeftArmHealth.Value < _statsContainer.LeftArmHealth.Value || decreasing || isRandomizing)
                SetStat(_viewStatsHolder.PlayerLeftArmStatView, _stats.LeftArmHealth.Value + value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
            if (_stats.RightArmHealth.Value < _statsContainer.RightArmHealth.Value || decreasing || isRandomizing)
                SetStat(_viewStatsHolder.PlayerRightArmStatView, _stats.RightArmHealth.Value + value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
            if (_stats.LeftLegHealth.Value < _statsContainer.LeftLegHealth.Value || decreasing || isRandomizing)
                SetStat(_viewStatsHolder.PlayerLeftLegStatView, _stats.LeftLegHealth.Value + value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
            if (_stats.RightLegHealth.Value < _statsContainer.RightLegHealth.Value || decreasing || isRandomizing)
                SetStat(_viewStatsHolder.PlayerRightLegStatView, _stats.RightLegHealth.Value + value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
        }

        private void ProcessStamina()
        {
            if (Math.Abs(_stats.Stamina.Value - _statsContainer.Stamina.Value) < 0.01f ||
                _playerInputState == PlayerInputState.Jump || _playerInputState == PlayerInputState.Sprint)
                return;

            float currentFatigue = _stats.Fatigue.Value / _statsContainer.Fatigue.Value;
            float currentHunger = _stats.Hunger.Value / _statsContainer.Hunger.Value;
            float currentThirst = _stats.Thirst.Value / _statsContainer.Thirst.Value;

            float currentRegen =
                _statsContainer.StaminaRegen.Value * (1 - _stats.Stamina.Value / _statsContainer.Stamina.Value) * currentFatigue * currentHunger * currentThirst;

            SetStat(_viewStatsHolder.PlayerStaminaStatView, _stats.Stamina.Value + currentRegen * Time.deltaTime);
            _stats.StaminaRegen.Value = currentRegen;
        }

        private void ProcessHunger()
        {
            if (_stats.Hunger.Value <= 0)
                return;

            float currentHungerDecrease = _stats.HungerDecrease.Value * _relativeSpeed;
            SetStat(_viewStatsHolder.PlayerHungerStatView, _stats.Hunger.Value - currentHungerDecrease * Time.deltaTime);
        }

        private void ProcessFatigue()
        {
            if (_stats.Fatigue.Value <= 0)
                return;

            float currentFatigueDecrease = _stats.FatigueDecrease.Value * _relativeSpeed;
            SetStat(_viewStatsHolder.PlayerFatigueStatView, _stats.Fatigue.Value - currentFatigueDecrease * Time.deltaTime);
        }

        private void ProcessThirst()
        {
            if (_stats.Thirst.Value <= 0)
                return;

            float currentThirstDecrease = _stats.ThirstDecrease.Value * _relativeSpeed;
            SetStat(_viewStatsHolder.PlayerThirstStatView, _stats.Thirst.Value - currentThirstDecrease * Time.deltaTime);
        }

        private void SetStat(BasePlayerStatView playerStatView, float value)
        {
            _statsByType[playerStatView.TargetStat].Value = value;
            playerStatView.Update();
        }
#endregion
#region Input
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
            SetStat(_viewStatsHolder.PlayerStaminaStatView, _stats.Stamina.Value - _staminaJumpCost);
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
            SetStat(_viewStatsHolder.PlayerStaminaStatView, _stats.Stamina.Value - _staminaSprintCost * Time.deltaTime);
        }
#endregion
#region Moving
        private void Footstep(AnimationEvent animationEvent)
        {
            if (!(animationEvent.animatorClipInfo.weight > 0.5f))
                return;
            if (_view.FootstepAudioClips.Length <= 0)
                return;
            int index = Random.Range(0, _view.FootstepAudioClips.Length);
            AudioSource.PlayClipAtPoint(_view.FootstepAudioClips[index], _view.transform.TransformPoint(_controller.center), _footstepAudioVolume);
        }

        private void Land(AnimationEvent animationEvent)
        {
            if (!(animationEvent.animatorClipInfo.weight > 0.5f))
                return;
            AudioSource.PlayClipAtPoint(_view.LandingAudioClip, _view.transform.TransformPoint(_controller.center), _footstepAudioVolume);
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
            Vector3 spherePosition = new Vector3(playerPos.x, playerPos.y - _gGroundedOffset,
                playerPos.z);
            _isGrounded = Physics.CheckSphere(spherePosition, _groundedRadius, _view.GroundLayers,
                QueryTriggerInteraction.Ignore);

            _playerInputState = _isGrounded ? PlayerInputState.Idle : PlayerInputState.Jump;

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
                _playerInputState = pendingInputState;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float multiplier = currentHorizontalSpeed < targetSpeed ? _speedUpChangeRate : _slowDownChangeRate;

            if (currentHorizontalSpeed < targetSpeed || currentHorizontalSpeed > targetSpeed)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * _inputMagnitude,
                    Time.deltaTime * multiplier);

                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
                _speed = targetSpeed;

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
                _speed *= 0.96f;

            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
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
#endregion
#region Cheats
        private void CHEAT_TimeSpeedUp(InputAction.CallbackContext obj)
        {
            _isTimeSpeedUp = !_isTimeSpeedUp;
            Time.timeScale = _isTimeSpeedUp ? 10f : 1f;
        }

        private void CHEAT_Damage(InputAction.CallbackContext obj)
        {
            SetAllHealths(isRandomizing: true);
        }
#endregion
    }
}
using Effects;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using WildIsland.Data;
using WildIsland.Utility;
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
        private PlayerData _statsDefault;
        private PlayerInputState _playerInputState;
        private PlayerInput _playerInput;
        private InputMap _inputMap;
        private Animator _animator;
        private CharacterController _characterController;
        private Dictionary<PlayerStat, BasePlayerStatView> _statViewPairs;
        private List<BaseEffect> _effects;
        private BaseEffect[] _nativeEffects;

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

        private const float _inAirVelocityReduction = 0.98f;
        private const float _inputMagnitude = 1f;
        private const float _speedUpChangeRate = 1.5f;
        private const float _slowDownChangeRate = 10f;
        private const float _rotationSmoothTime = 0.12f;
        private const float _footstepAudioVolume = 0.5f;
        private const float _jumpHeight = 1.7f;
        private const float _gravity = -15f;
        private const float _jumpTimeout = 0.1f;
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
            => _statsDefault = ((PlayerDataContainer)container).Default;

        public void Initialize()
        {
            InitView();
            InitInputActions();
            SetData();
        }

        public void Tick()
        {
            UpdateStats();
            ProcessEffects();
            GroundedCheck();

            ReadInput();
            Move();
            JumpAndGravity();
        }

        public void LateTick()
            => CameraRotation();

        public void Dispose()
        {
            _inputMap.Dispose();
            //todo remove default
            _data.Save(_statsDefault);
        }

#region InitActions
        private void SetData()
        {
            _playerInputState = PlayerInputState.Idle;
            _data = new DbValue<PlayerData>("PlayerData", _statsDefault);

            _jumpTimeoutDelta = _jumpTimeout;
            _fallTimeoutDelta = _fallTimeout;

            _staminaJumpCost = _view.StaminaJumpCost / _statsDefault.Stamina.Value * 100;
            _staminaSprintCost = _view.StaminaSprintCost / _statsDefault.Stamina.Value * 100;

            _moveSpeed = _data.Value.RegularSpeed.Value;
            _sprintSpeed = _data.Value.SprintSpeed.Value;

            _statViewPairs = new Dictionary<PlayerStat, BasePlayerStatView>
            {
                { _stats.HeadHealth, _viewStatsHolder.PlayerHeadStatView },
                { _stats.BodyHealth, _viewStatsHolder.PlayerBodyStatView },
                { _stats.LeftArmHealth, _viewStatsHolder.PlayerLeftArmStatView },
                { _stats.RightArmHealth, _viewStatsHolder.PlayerRightArmStatView },
                { _stats.LeftLegHealth, _viewStatsHolder.PlayerLeftLegStatView },
                { _stats.RightLegHealth, _viewStatsHolder.PlayerRightLegStatView },
                { _stats.Stamina, _viewStatsHolder.PlayerStaminaStatView },
                { _stats.Hunger, _viewStatsHolder.PlayerHungerStatView },
                { _stats.Thirst, _viewStatsHolder.PlayerThirstStatView },
                { _stats.Fatigue, _viewStatsHolder.PlayerFatigueStatView },
            };

            _effects = new List<BaseEffect>();
            _nativeEffects = Array.Empty<BaseEffect>();

            SetStatViews();
        }

        private void SetStatViews()
        {
            _viewStatsHolder.PlayerBodyStatView.SetRefs(_stats.BodyHealth, _statsDefault.BodyHealth);
            _viewStatsHolder.PlayerHeadStatView.SetRefs(_stats.HeadHealth, _statsDefault.HeadHealth);
            _viewStatsHolder.PlayerLeftArmStatView.SetRefs(_stats.LeftLegHealth, _statsDefault.LeftLegHealth);
            _viewStatsHolder.PlayerRightArmStatView.SetRefs(_stats.RightArmHealth, _statsDefault.RightArmHealth);
            _viewStatsHolder.PlayerLeftLegStatView.SetRefs(_stats.LeftLegHealth, _statsDefault.LeftLegHealth);
            _viewStatsHolder.PlayerRightLegStatView.SetRefs(_stats.RightLegHealth, _statsDefault.RightLegHealth);
            _viewStatsHolder.PlayerStaminaStatView.SetRefs(_stats.Stamina, _statsDefault.Stamina);
            _viewStatsHolder.PlayerHungerStatView.SetRefs(_stats.Hunger, _statsDefault.Hunger);
            _viewStatsHolder.PlayerThirstStatView.SetRefs(_stats.Thirst, _statsDefault.Thirst);
            _viewStatsHolder.PlayerFatigueStatView.SetRefs(_stats.Fatigue, _statsDefault.Fatigue);
        }

        private void InitInputActions()
        {
            _playerInput = new PlayerInput();
            _inputMap = new InputMap();
            _inputMap.Enable();

            _inputMap.Player.Jump.performed += OnJumpPerformed;
            _inputMap.Player.CHEAT_Time.started += CHEAT_TimeSpeedUp;
            _inputMap.Player.CHEAT_Damage.started += CHEAT_Damage;
            _inputMap.Player.CHEAT_FrameRate.started += CHEAT_FrameRateChange;
            _inputMap.Player.CHEAT_PeriodicEffect.started += CHEAT_PeriodicEffectApply;
            _inputMap.Player.CHEAT_TemporaryEffect.started += CHEAT_TemporaryEffectApply;
        }

        private void InitView()
        {
            _cinemachineTargetYaw = _view.CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = _view.TryGetComponent(out _animator);
            _characterController = _view.GetComponent<CharacterController>();

            _view.SetOnLandCallback(Land);
            _view.SetOnFootStepCallback(Footstep);
            _view.SetEffectCallbacks(EffectApply, EffectRemove);
        }
#endregion
#region EffectProcessor
        private void ProcessEffects()
        {
            _nativeEffects = new BaseEffect[_effects.Count];
            _effects.CopyTo(_nativeEffects);
            
            foreach (BaseEffect effect in _nativeEffects)
            {
                if (effect is PermanentEffect permanentEffect)
                {
                    if (permanentEffect.Process())
                        SetStats(permanentEffect.Apply(_stats));
                }
                else 
                {
                    if (effect.Process())
                        SetStats(effect.Apply(_stats));
                    else if (effect.IsExecuted)
                        EffectRemove(effect.GetType());
                }
            }
        }

        private void EffectApply(BaseEffect effect)
            => _effects.Add(effect);

        private void EffectRemove(Type effectType)
        {
            BaseEffect target = _effects.Find(x => x.GetType() == effectType);
            target.Remove(_stats);
            _effects.Remove(target);
        }

        private void SetStats(PlayerStat[] affectedStats)
        {
            if (affectedStats == null)
                return;
            foreach (PlayerStat stat in affectedStats)
                SetStat(stat, forceDebugShow: true);
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
                hungerMod = _statsDefault.HealthRegenHungerStage1.Value;
            else if (currentHunger <= _view.HungerRegenStage2Range.y && currentHunger >= _view.HungerRegenStage2Range.x)
                hungerMod = _statsDefault.HealthRegenHungerStage2.Value;
            else if (currentHunger <= _view.HungerRegenStage3Range.y && currentHunger >= _view.HungerRegenStage3Range.x)
                hungerMod = _statsDefault.HealthRegenHungerStage3.Value;
            else if (currentHunger <= _view.HungerRegenStage4Range.y)
                hungerMod = _statsDefault.HealthRegenHungerStage4.Value;

            if (currentThirst <= _view.ThirstRegenStage1Range.y && currentThirst >= _view.ThirstRegenStage1Range.x)
                thirstMod = _statsDefault.HealthRegenThirstStage1.Value;
            else if (currentThirst <= _view.ThirstRegenStage2Range.y && currentThirst >= _view.ThirstRegenStage2Range.x)
                thirstMod = _statsDefault.HealthRegenThirstStage2.Value;
            else if (currentThirst <= _view.ThirstRegenStage3Range.y && currentThirst >= _view.ThirstRegenStage3Range.x)
                thirstMod = _statsDefault.HealthRegenThirstStage3.Value;
            else if (currentThirst <= _view.ThirstRegenStage4Range.y)
                thirstMod = _statsDefault.HealthRegenThirstStage4.Value;

            float currentRegen = _stats.HealthRegen.Value + hungerMod + thirstMod;
            SetAllHealths(currentRegen * Time.deltaTime);
        }

        private void SetAllHealths(float value = 0f, bool isRandomizing = false)
        {
            bool decreasing = value < 0;

            if (_stats.HeadHealth.Value < _statsDefault.HeadHealth.Value || decreasing || isRandomizing)
                SetStat(_stats.HeadHealth, value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
            if (_stats.BodyHealth.Value < _statsDefault.BodyHealth.Value || decreasing || isRandomizing)
                SetStat(_stats.BodyHealth, value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
            if (_stats.LeftArmHealth.Value < _statsDefault.LeftArmHealth.Value || decreasing || isRandomizing)
                SetStat(_stats.LeftArmHealth, value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
            if (_stats.RightArmHealth.Value < _statsDefault.RightArmHealth.Value || decreasing || isRandomizing)
                SetStat(_stats.RightArmHealth, value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
            if (_stats.LeftLegHealth.Value < _statsDefault.LeftLegHealth.Value || decreasing || isRandomizing)
                SetStat(_stats.LeftLegHealth, value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
            if (_stats.RightLegHealth.Value < _statsDefault.RightLegHealth.Value || decreasing || isRandomizing)
                SetStat(_stats.RightLegHealth, value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
        }

        private void ProcessStamina()
        {
            if (Math.Abs(_stats.Stamina.Value - _statsDefault.Stamina.Value) < 0.01f ||
                _playerInputState == PlayerInputState.Jump || _playerInputState == PlayerInputState.Sprint)
                return;

            float currentFatigue = _stats.Fatigue.Value / _statsDefault.Fatigue.Value;
            float currentHunger = _stats.Hunger.Value / _statsDefault.Hunger.Value;
            float currentThirst = _stats.Thirst.Value / _statsDefault.Thirst.Value;

            float currentRegen =
                _stats.StaminaRegen.Value * (1 - _stats.Stamina.Value / _statsDefault.Stamina.Value) * currentFatigue * currentHunger * currentThirst;

            SetStat(_stats.Stamina, currentRegen * Time.deltaTime);
        }

        private void ProcessHunger()
        {
            if (_stats.Hunger.Value <= 0)
                return;

            float currentHungerDecrease = _stats.HungerDecrease.Value + _relativeSpeed;
            SetStat(_stats.Hunger, -currentHungerDecrease * Time.deltaTime, true);
        }

        private void ProcessFatigue()
        {
            if (_stats.Fatigue.Value <= 0)
                return;

            float currentFatigueDecrease = _stats.FatigueDecrease.Value + _relativeSpeed;
            SetStat(_stats.Fatigue, -currentFatigueDecrease * Time.deltaTime, true);
        }

        private void ProcessThirst()
        {
            if (_stats.Thirst.Value <= 0)
                return;

            float currentThirstDecrease = _stats.ThirstDecrease.Value + _relativeSpeed;
            SetStat(_stats.Thirst, -currentThirstDecrease * Time.deltaTime, true);
        }

        private void SetStat(PlayerStat stat, float value = 0, bool forceDebugShow = false)
        {
            stat.Value += value;
            if (stat.Value < 0)
                stat.Value = 0;
            _statViewPairs.TryGetValue(stat, out BasePlayerStatView statView);
            if (statView == null)
                return;
            float perSecondsMult = Mathf.Abs(value) < 1 ? Time.deltaTime : 1;
            statView.UpdateDebugValue(value / perSecondsMult, forceDebugShow);
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
            SetStat(_stats.Stamina, -_staminaJumpCost, true);
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
            SetStat(_stats.Stamina, -_staminaSprintCost * Time.deltaTime, true);
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
            AudioSource.PlayClipAtPoint(_view.FootstepAudioClips[index], _view.transform.TransformPoint(_characterController.center), _footstepAudioVolume);
        }

        private void Land(AnimationEvent animationEvent)
        {
            if (!(animationEvent.animatorClipInfo.weight > 0.5f))
                return;
            AudioSource.PlayClipAtPoint(_view.LandingAudioClip, _view.transform.TransformPoint(_characterController.center), _footstepAudioVolume);
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

            float currentHorizontalSpeed = new Vector3(_characterController.velocity.x, 0.0f, _characterController.velocity.z).magnitude;
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
                _speed *= _inAirVelocityReduction;

            _characterController.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
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
            Time.timeScale = _isTimeSpeedUp ? 100f : 1f;
        }

        private void CHEAT_Damage(InputAction.CallbackContext obj)
            => SetAllHealths(isRandomizing: true);

        private void CHEAT_FrameRateChange(InputAction.CallbackContext obj)
        {
            _isFrameRate60 = !_isFrameRate60;
            Application.targetFrameRate = _isFrameRate60 ? 60 : 150;
        }

        private void CHEAT_TemporaryEffectApply(InputAction.CallbackContext obj)
        {
            TestTemporaryEffect test = new TestTemporaryEffect(5f, TempApply, TempRemove);
            _effects.Add(test);
        }

        private AffectedStats tempAffected;

        private PlayerStat[] TempApply(PlayerData playerData)
        {
            tempAffected = new AffectedStats { new Tuple<PlayerStat, float>(_stats.HungerDecrease, 3) };
            return tempAffected.ApplyReturnStats;
        }

        private PlayerStat[] TempRemove(PlayerData arg)
            => tempAffected.RevertReturnStats;

        private AffectedStats periodAffected;

        private void CHEAT_PeriodicEffectApply(InputAction.CallbackContext obj)
        {
            TestPeriodicEffect test = new TestPeriodicEffect(1f, 3f, PeriodicApply, PeriodicRemove);
            _effects.Add(test);
        }

        private PlayerStat[] PeriodicApply(PlayerData arg)
        {
            periodAffected = new AffectedStats { new Tuple<PlayerStat, float>(_stats.HeadHealth, -10) };
            return periodAffected.ApplyReturnStats;
        }

        private PlayerStat[] PeriodicRemove(PlayerData arg)
            => periodAffected.RevertReturnStats;
#endregion
    }
}
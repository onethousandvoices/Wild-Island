using System;
using UnityEngine;
using WildIsland.Data;
using WildIsland.Views;
using Zenject;
using Random = UnityEngine.Random;

namespace WildIsland.Controllers
{
    public class PlayerController : IInitializable, ITickable, ILateTickable, IDisposable, IGDConsumer
    {
        [Inject] private Camera _mainCamera;
        [Inject] private PlayerView _view;

        private DbValue<PlayerData> _data;
        private PlayerData _dataContainer;
        private Animator _animator;
        private CharacterController _controller;
        private Core.PlayerInput _playerInput;

        private bool _hasAnimator;
        private bool _lockCameraPosition;
        private bool _grounded = true;

        private float _moveSpeed;
        private float _sprintSpeed;
        private float _cameraAngleOverride;
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        private float _speed;
        private float _animationBlend;
        private float _targetRotation;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        private const float _inputMagnitude = 1f;
        private const float _speedUpChangeRate = 5f;
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

        public Type ContainerType => typeof(PlayerDataContainer);

        public void AcquireGameData(IPartialGameDataContainer container)
            => _dataContainer = ((PlayerDataContainer)container).Default;

        public void Initialize()
        {
            _data = new DbValue<PlayerData>("PlayerData", _dataContainer);

            SetData();

            _cinemachineTargetYaw = _view.CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = _view.TryGetComponent(out _animator);
            _controller = _view.GetComponent<CharacterController>();
            _playerInput = _view.GetComponent<Core.PlayerInput>();

            _view.SetOnLandCallback(Land);
            _view.SetOnFootStepCallback(Footstep);
        }

        public void Tick()
        {
            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        public void LateTick()
            => CameraRotation();

        public void Dispose()
        {
            _data.Save();
        }

        private void SetData()
        {
            _jumpTimeoutDelta = _jumpTimeout;
            _fallTimeoutDelta = _fallTimeout;

            _moveSpeed = _data.Value.RegularSpeed;
            _sprintSpeed = _data.Value.SprintSpeed;
        }

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
            if (_grounded)
            {
                _fallTimeoutDelta = _fallTimeout;

                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                if (_verticalVelocity < 0.0f)
                    _verticalVelocity = -2f;

                if (_playerInput.Jump && _jumpTimeoutDelta <= 0.0f)
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
            _grounded = Physics.CheckSphere(spherePosition, _groundedRadius, _view.GroundLayers,
                QueryTriggerInteraction.Ignore);

            if (_hasAnimator)
                _animator.SetBool(_animIDGrounded, _grounded);
        }

        private void Move()
        {
            float targetSpeed = _playerInput.Sprint ? _sprintSpeed : _moveSpeed;

            if (_playerInput.Move == Vector2.zero)
                targetSpeed = 0.0f;

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

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * multiplier);
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

            if (!_grounded)
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
            if (_playerInput.Look.sqrMagnitude >= _threshold && !_lockCameraPosition)
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
    }
}
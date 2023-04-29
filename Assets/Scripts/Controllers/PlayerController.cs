using Data;
using System;
using UnityEngine;
using WildIsland.Views;
using Zenject;
using Random = UnityEngine.Random;

namespace WildIsland.Controllers
{
    public class PlayerController : IInitializable, ITickable, ILateTickable/*, IGDConsumer*/
    {
        [Inject] private Camera _mainCamera;
        [Inject] private PlayerView _view;

        private Animator _animator;
        private CharacterController _controller;
        private Core.PlayerInput _playerInput;

        private bool _hasAnimator;

        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private const float _terminalVelocity = 53.0f;
        private const float _threshold = 0.01f;
        private const float deltaTimeMultiplier = 1f;

        // public Type ContainerType { get; }

        // public void AcquireGameData(IPartialGameDataContainer container) { }

        public void Initialize()
        {
            _cinemachineTargetYaw = _view.CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = _view.TryGetComponent(out _animator);
            _controller = _view.GetComponent<CharacterController>();
            _playerInput = _view.GetComponent<Core.PlayerInput>();

            AssignAnimationIDs();

            _jumpTimeoutDelta = _view.JumpTimeout;
            _fallTimeoutDelta = _view.FallTimeout;
            
            _view.SetOnLandCallback(Land);
            _view.SetOnFootStepCallback(Footstep);
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void Footstep(AnimationEvent animationEvent)
        {
            if (!(animationEvent.animatorClipInfo.weight > 0.5f))
                return;
            if (_view.FootstepAudioClips.Length <= 0)
                return;
            int index = Random.Range(0, _view.FootstepAudioClips.Length);
            AudioSource.PlayClipAtPoint(_view.FootstepAudioClips[index], _view.transform.TransformPoint(_controller.center), _view.FootstepAudioVolume);
        }

        private void Land(AnimationEvent animationEvent)
        {
            if (!(animationEvent.animatorClipInfo.weight > 0.5f))
                return;
            AudioSource.PlayClipAtPoint(_view.LandingAudioClip, _view.transform.TransformPoint(_controller.center), _view.FootstepAudioVolume);
        }

        public void Tick()
        {
            _hasAnimator = _view.TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        private void JumpAndGravity()
        {
            if (_view.Grounded)
            {
                _fallTimeoutDelta = _view.FallTimeout;

                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                if (_verticalVelocity < 0.0f)
                    _verticalVelocity = -2f;

                if (_playerInput.Jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(_view.JumpHeight * -2f * _view.Gravity);

                    if (_hasAnimator)
                        _animator.SetBool(_animIDJump, true);
                }

                if (_jumpTimeoutDelta >= 0.0f)
                    _jumpTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _jumpTimeoutDelta = _view.JumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                    _fallTimeoutDelta -= Time.deltaTime;
                else if (_hasAnimator)
                    _animator.SetBool(_animIDFreeFall, true);
                _playerInput.ResetJump();
            }

            if (_verticalVelocity < _terminalVelocity)
                _verticalVelocity += _view.Gravity * Time.deltaTime;
        }

        private void GroundedCheck()
        {
            Vector3 playerPos = _view.transform.position;
            Vector3 spherePosition = new Vector3(playerPos.x, playerPos.y - _view.GroundedOffset,
                playerPos.z);
            _view.SetGrounded(Physics.CheckSphere(spherePosition, _view.GroundedRadius, _view.GroundLayers,
                QueryTriggerInteraction.Ignore));

            if (_hasAnimator)
                _animator.SetBool(_animIDGrounded, _view.Grounded);
        }

        private void Move()
        {
            float targetSpeed = _playerInput.Sprint ? _view.SprintSpeed : _view.MoveSpeed;

            if (_playerInput.Move == Vector2.zero)
                targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            const float speedOffset = 0.1f;
            const float inputMagnitude = 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * _view.SpeedChangeRate);

                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
                _speed = targetSpeed;

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * _view.SpeedChangeRate);
            if (_animationBlend < 0.01f)
                _animationBlend = 0f;

            Vector3 inputDirection = new Vector3(_playerInput.Move.x, 0.0f, _playerInput.Move.y).normalized;

            if (_playerInput.Move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;

                float rotation = Mathf.SmoothDampAngle(_view.transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    _view.RotationSmoothTime);

                _view.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            if (!_hasAnimator)
                return;
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }

        public void LateTick()
            => CameraRotation();

        private void CameraRotation()
        {
            if (_playerInput.Look.sqrMagnitude >= _threshold && !_view.LockCameraPosition)
            {
                _cinemachineTargetYaw += _playerInput.Look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _playerInput.Look.y * deltaTimeMultiplier;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _view.BottomClamp, _view.TopClamp);

            _view.CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + _view.CameraAngleOverride,
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
}
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using WildIsland.Utility;
using WildIsland.Views;
using Zenject;
using CameraType = WildIsland.Views.CameraType;

namespace WildIsland.Processors
{
    public class PlayerCameraProcessor : BaseProcessor, IInitializable, ILatePlayerProcessor, IPlayerCamera, IRMBListener, IDisposable
    {
        [Inject] private PlayerView _player;
        [Inject] private PlayerCamerasView _cameras;
        [Inject] private IPlayerState _playerState;

        private Vector2 _look;

        private const float _topClamp = 70f;
        private const float _bottomClamp = -30f;
        private const float _threshold = 0.01f;
        private const float _deltaTimeMultiplier = 1f;

        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        private float _cameraAngleOverride;

        private bool _isBlockCamera;

        public void Initialize()
        {
            _cinemachineTargetYaw = _player.CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            _playerState.InputStateChanged += OnInputStateChanged;
        }

        private void OnInputStateChanged()
        {
            _isBlockCamera = _playerState.InputState.HasFlagOptimized(InputState.BlockCamera);
            if (_isBlockCamera)
                _cameras.SwitchCamera(CameraType.Base);
        }

        public void LateTick()
            => CameraRotation();

        public void SetLookVector(Vector2 look)
            => _look = look;

        public void OnRMBStarted(InputAction.CallbackContext obj)
        {
            if (_isBlockCamera)
                return;
            _cameras.SwitchCamera(CameraType.Aim);
        }

        public void OnRMBCanceled(InputAction.CallbackContext obj)
        {
            if (_isBlockCamera)
                return;
            _cameras.SwitchCamera(CameraType.Base);
        }

        private void CameraRotation()
        {
            if (_look.sqrMagnitude >= _threshold)
            {
                _cinemachineTargetYaw += _look.x * _deltaTimeMultiplier;
                _cinemachineTargetPitch += _look.y * _deltaTimeMultiplier;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _bottomClamp, _topClamp);

            _player.CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + _cameraAngleOverride,
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

        public void Dispose()
            => _playerState.InputStateChanged -= OnInputStateChanged;
    }

    public interface IPlayerCamera
    {
        public void SetLookVector(Vector2 look);
    }
}
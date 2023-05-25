using Cinemachine;
using System;
using UnityEngine;

namespace WildIsland.Views
{
    public enum CameraType : byte
    {
        Base,
        Aim
    }

    public class PlayerCamerasView : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _baseCamera;
        [SerializeField] private CinemachineVirtualCamera _aimCamera;

        public void SwitchCamera(CameraType type)
        {
            switch (type)
            {
                case CameraType.Base:
                    _baseCamera.Priority = 10;
                    _aimCamera.Priority = 0;
                    break;
                case CameraType.Aim:
                    _baseCamera.Priority = 0;
                    _aimCamera.Priority = 10;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
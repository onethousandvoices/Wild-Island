using System;
using UnityEngine;
using WildIsland.Core;

namespace WildIsland.Views
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerInput), typeof(Animator))]
    public class PlayerView : MonoBehaviour
    {
        [field: SerializeField] public float MoveSpeed { get; private set; } = 2.0f;
        [field: SerializeField] public float SprintSpeed { get; private set; } = 5.335f;

        [field: SerializeField, Range(0.0f, 0.3f)] public float RotationSmoothTime { get; private set; } = 0.12f;
        [field: SerializeField] public float SpeedChangeRate { get; private set; } = 10.0f;

        [field: SerializeField] public AudioClip LandingAudioClip { get; private set; }
        [field: SerializeField] public AudioClip[] FootstepAudioClips { get; private set; }
        [field: SerializeField, Range(0, 1)] public float FootstepAudioVolume { get; private set; } = 0.5f;

        [field: SerializeField] public float JumpHeight = 1.2f;
        [field: SerializeField] public float Gravity { get; private set; } = -15.0f;

        [field: SerializeField] public float JumpTimeout { get; private set; } = 0.50f;
        [field: SerializeField] public float FallTimeout { get; private set; } = 0.15f;

        [field: SerializeField] public bool Grounded { get; private set; } = true;
        [field: SerializeField] public float GroundedOffset { get; private set; } = -0.14f;
        [field: SerializeField] public float GroundedRadius { get; private set; } = 0.28f;
        [field: SerializeField] public LayerMask GroundLayers { get; private set; }

        [field: SerializeField] public GameObject CinemachineCameraTarget { get; private set; }
        [field: SerializeField] public float TopClamp { get; private set; } = 70.0f;
        [field: SerializeField] public float BottomClamp { get; private set; } = -30.0f;
        [field: SerializeField] public float CameraAngleOverride { get; private set; }
        [field: SerializeField] public bool LockCameraPosition { get; private set; }

        private Action<AnimationEvent> OnLandCallback;
        private Action<AnimationEvent> OnFootStepCallback;

        public void SetOnLandCallback(Action<AnimationEvent> callback)
            => OnLandCallback = callback;

        public void SetOnFootStepCallback(Action<AnimationEvent> callback)
            => OnFootStepCallback = callback;

        public void SetGrounded(bool state)
            => Grounded = state;
        
        private void OnFootstep(AnimationEvent animationEvent)
            => OnFootStepCallback?.Invoke(animationEvent);

        private void OnLand(AnimationEvent animationEvent)
            => OnLandCallback?.Invoke(animationEvent);
        
    }
}
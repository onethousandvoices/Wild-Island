using Effects;
using NaughtyAttributes;
using System;
using UnityEngine;

namespace WildIsland.Views
{
    public class PlayerView : MonoBehaviour, IEffectReceiver
    {
        [field: SerializeField, HorizontalLine(color: EColor.Blue), MinMaxSlider(0f, 100f)] public Vector2 HungerRegenStage1Range { get; private set; }
        [field: SerializeField, MinMaxSlider(0f, 100f)] public Vector2 HungerRegenStage2Range { get; private set; }
        [field: SerializeField, MinMaxSlider(0f, 100f)] public Vector2 HungerRegenStage3Range { get; private set; }
        [field: SerializeField, MinMaxSlider(0f, 100f)] public Vector2 HungerRegenStage4Range { get; private set; }
        [field: SerializeField, HorizontalLine(color: EColor.Blue), MinMaxSlider(0f, 100f)] public Vector2 ThirstRegenStage1Range { get; private set; }
        [field: SerializeField, MinMaxSlider(0f, 100f)] public Vector2 ThirstRegenStage2Range { get; private set; }
        [field: SerializeField, MinMaxSlider(0f, 100f)] public Vector2 ThirstRegenStage3Range { get; private set; }
        [field: SerializeField, MinMaxSlider(0f, 100f)] public Vector2 ThirstRegenStage4Range { get; private set; }
        [field: SerializeField, HorizontalLine(color: EColor.Blue), Range(1f, 50f)] public float StaminaJumpCost { get; private set; }
        [field: SerializeField, Range(1f, 50f)] public float StaminaSprintCost { get; private set; }
        [field: SerializeField, Range(1f, 10f)] public float JumpHeight { get; private set; }
        [field: SerializeField, Range(0.5f, 1f)] public float InAirVelocityReduction { get; private set; }
        [field: SerializeField, Range(0.5f, 1f)] public float HorizontalVelocityReduction { get; private set; }
        [field: SerializeField, Range(0.5f, 1f)] public float BackwardsVelocityReduction { get; private set; }
        [field: SerializeField, Range(1, 100)] public int FallDamagePerHeight { get; private set; }

        [field: SerializeField, HorizontalLine(color: EColor.Red)] public Animator Animator { get; private set; }
        [field: SerializeField] public PlayerViewEventsReceiver EventsReceiver { get; private set; }
        [field: SerializeField] public AudioClip LandingAudioClip { get; private set; }
        [field: SerializeField] public AudioClip[] FootstepAudioClips { get; private set; }
        [field: SerializeField] public LayerMask GroundLayers { get; private set; }
        [field: SerializeField] public GameObject CinemachineCameraTarget { get; private set; }
        [field: SerializeField] public PhysicMaterial SlipperyMaterial { get; private set; }
        [field: SerializeField] public PhysicMaterial FrictionMaterial { get; private set; }

        private Rigidbody _rb;
        private Vector3 _spherePosition;

        private float _sphereRadius;
        
        public Action<BaseEffect> OnEffectApplied { get; private set; }
        public Action<Type> OnEffectRemoved { get; private set; }

        public Rigidbody Rb => _rb ??= GetComponent<Rigidbody>();

        public void SetEffectCallbacks(Action<BaseEffect> apply, Action<Type> remove)
        {
            OnEffectApplied = apply;
            OnEffectRemoved = remove;
        }

        // public void SetOnLandCallback(Action<AnimationEvent> callback)
            // => OnLandCallback = callback;

        public void SetOnFootStepCallback(Action<AnimationEvent> callback)
            => EventsReceiver.SetOnFootstepCallback(callback);


        public void SetGroundCheckSphereParams(Vector3 position, float radius)
        {
            _spherePosition = position;
            _sphereRadius = radius;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(_spherePosition, _sphereRadius);
        }
    }
    
    public interface IEffectReceiver
    {
        public Action<BaseEffect> OnEffectApplied { get; }
        public Action<Type> OnEffectRemoved { get; }
    }
}
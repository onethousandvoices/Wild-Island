using Effects;
using NaughtyAttributes;
using System;
using UnityEngine;

namespace WildIsland.Views
{
    [RequireComponent(typeof(Rigidbody), typeof(Animator))]
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
        [field: SerializeField, HorizontalLine(color: EColor.Red)] public AudioClip LandingAudioClip { get; private set; }
        [field: SerializeField] public AudioClip[] FootstepAudioClips { get; private set; }
        [field: SerializeField] public LayerMask GroundLayers { get; private set; }
        [field: SerializeField] public GameObject CinemachineCameraTarget { get; private set; }
        [field: SerializeField] public PhysicMaterial SlipperyMaterial { get; private set; }
        [field: SerializeField] public PhysicMaterial FrictionMaterial { get; private set; }

        private void OnDrawGizmos()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - 0.05f, transform.position.z);
            Gizmos.DrawSphere(spherePosition, 0.15f);
        }

        private Rigidbody _rb;
        
        private Action<AnimationEvent> OnLandCallback;
        private Action<AnimationEvent> OnFootStepCallback;
        
        public Action<BaseEffect> OnEffectApplied { get; private set; }
        public Action<Type> OnEffectRemoved { get; private set; }
        
        public Rigidbody Rb => _rb ??= GetComponent<Rigidbody>();
        
        public void SetEffectCallbacks(Action<BaseEffect> apply, Action<Type> remove)
        {
            OnEffectApplied = apply;
            OnEffectRemoved = remove;
        }

        public void SetOnLandCallback(Action<AnimationEvent> callback)
            => OnLandCallback = callback;

        public void SetOnFootStepCallback(Action<AnimationEvent> callback)
            => OnFootStepCallback = callback;

        private void OnFootstep(AnimationEvent animationEvent)
            => OnFootStepCallback?.Invoke(animationEvent);

        private void OnLand(AnimationEvent animationEvent)
            => OnLandCallback?.Invoke(animationEvent);
    }

    public interface IEffectReceiver
    {
        public Action<BaseEffect> OnEffectApplied { get; }
        public Action<Type> OnEffectRemoved { get; }
    }
}
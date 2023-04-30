using System;
using UnityEngine;
using WildIsland.Core;

namespace WildIsland.Views
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerInput), typeof(Animator))]
    public class PlayerView : MonoBehaviour
    {   
        [field: SerializeField] public AudioClip LandingAudioClip { get; private set; }            
        [field: SerializeField] public AudioClip[] FootstepAudioClips { get; private set; }        
        [field: SerializeField] public LayerMask GroundLayers { get; private set; }                
        [field: SerializeField] public GameObject CinemachineCameraTarget { get; private set; }    
        
        private Action<AnimationEvent> OnLandCallback;
        private Action<AnimationEvent> OnFootStepCallback;

        public void SetOnLandCallback(Action<AnimationEvent> callback)
            => OnLandCallback = callback;

        public void SetOnFootStepCallback(Action<AnimationEvent> callback)
            => OnFootStepCallback = callback;
        
        private void OnFootstep(AnimationEvent animationEvent)
            => OnFootStepCallback?.Invoke(animationEvent);

        private void OnLand(AnimationEvent animationEvent)
            => OnLandCallback?.Invoke(animationEvent);
        
    }
}
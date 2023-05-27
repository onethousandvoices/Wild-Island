using System;
using UnityEngine;

namespace WildIsland.Views
{
    public class PlayerViewEventsReceiver : MonoBehaviour
    {
        private Action<AnimationEvent> _onFootstep;
        
        public void SetOnFootstepCallback(Action<AnimationEvent>callback)
            => _onFootstep = callback;

        public void UnityEvent_Footstep(AnimationEvent step)
            => _onFootstep?.Invoke(step);
    }
}
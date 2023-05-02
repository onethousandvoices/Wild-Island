using UnityEngine;

namespace Effects
{
    public class TemporaryEffect : BaseEffect
    {
        private readonly float _duration;
        private float _currentDuration;
        
        public TemporaryEffect(float duration, params PlayerDataEffect[] effects) : base(effects)
            => _duration = duration;

        public override bool IsApplying()
        {
            _currentDuration -= Time.deltaTime;
            return _currentDuration > 0;
        }
    }
}
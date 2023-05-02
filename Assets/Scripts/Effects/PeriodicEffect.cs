using UnityEngine;

namespace Effects
{
    public class PeriodicEffect : BaseEffect
    {
        private readonly float _period;
        private float _currentCooldown;

        protected PeriodicEffect(float period, params PlayerDataEffect[] effects) : base(effects)
        {
            _period = period;
            _currentCooldown = _period;
        }

        public override bool IsApplying()
        {
            _currentCooldown -= Time.deltaTime;
            if (_currentCooldown > 0)
                return false;
            _currentCooldown = _period;
            return true;
        }
    }
}
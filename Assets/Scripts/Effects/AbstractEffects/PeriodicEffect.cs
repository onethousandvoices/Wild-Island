using System;
using UnityEngine;
using WildIsland.Data;

namespace Effects
{
    public abstract class PeriodicEffect : BaseEffect
    {
        private readonly float _period;
        private float _duration;

        protected PeriodicEffect(float period, float duration, Action<PlayerData> apply) : base(apply)
        {
            _period = period;
            _duration = duration;
            CurrentCooldown = _period;
        }

        public override bool Process()
        {
            if (InstantApply())
                return true;
            
            if (_duration > 0)
                _duration -= Time.deltaTime;
            CurrentCooldown -= Time.deltaTime;
            if (CurrentCooldown > 0)
                return false;
            if (_duration < 0)
            {
                IsExecuted = true;
                return false;
            }
            CurrentCooldown = _period;
            return true;
        }
    }
}
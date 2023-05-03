using System;
using WildIsland.Data;

namespace Effects
{
    public class PeriodicEffect : BaseEffect
    {
        private readonly float _period;
        private float _currentCooldown;

        public PeriodicEffect(float period, Func<PlayerData,  PlayerStat[]> apply, Func<PlayerData,  PlayerStat[]> remove) : base(apply, remove)
        {
            _period = period;
            _currentCooldown = _period;
        }
    }
}
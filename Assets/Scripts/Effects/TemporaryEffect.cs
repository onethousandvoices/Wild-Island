using System;
using WildIsland.Data;

namespace Effects
{
    public class TemporaryEffect : BaseEffect
    {
        private float _currentDuration;

        public TemporaryEffect(float duration, Func<PlayerData, PlayerStat[]> apply, Func<PlayerData, PlayerStat[]> remove) : base(apply, remove)
            => _currentDuration = duration;
    }
}
using System;
using WildIsland.Data;

namespace Effects
{
    public class TestPeriodicEffect : PeriodicEffect
    {
        public TestPeriodicEffect(float period, float duration, Func<PlayerData, PlayerStat[]> apply, Func<PlayerData, PlayerStat[]> remove) : base(period, duration, apply, remove) { }
    }
}
using System;
using WildIsland.Data;

namespace Effects
{
    public class TestPeriodicEffect : PeriodicEffect
    {
        public TestPeriodicEffect(float period, float duration, Action<PlayerData> apply = null) : base(period, duration, apply) { }
    }
}
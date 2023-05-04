using System;
using WildIsland.Data;

namespace Effects
{
    public class TestPeriodicEffect : PeriodicEffect
    {
        public TestPeriodicEffect(float period, float duration, Action<PlayerData> apply) : base(period, duration, apply) { }
    }
}
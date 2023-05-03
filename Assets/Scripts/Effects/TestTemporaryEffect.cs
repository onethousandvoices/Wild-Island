using System;
using WildIsland.Data;

namespace Effects
{
    public class TestTemporaryEffect : TemporaryEffect
    {
        public TestTemporaryEffect(float duration, Func<PlayerData, PlayerStat[]> apply, Func<PlayerData, PlayerStat[]> remove) : base(duration, apply, remove) { }
    }
}
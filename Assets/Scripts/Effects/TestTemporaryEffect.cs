using System;
using WildIsland.Data;

namespace Effects
{
    public class TestTemporaryEffect : TemporaryEffect
    {
        public TestTemporaryEffect(float duration, Action<PlayerData> apply = null) : base(duration, apply) { }
    }
}
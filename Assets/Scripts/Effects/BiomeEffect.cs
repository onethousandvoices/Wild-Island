using System;
using WildIsland.Data;

namespace Effects
{
    public class BiomeEffect : PermanentEffect
    {
        public BiomeEffect(Action<PlayerData> apply) : base(apply) { }
    }
}
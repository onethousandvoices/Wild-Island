using System;
using WildIsland.Data;

namespace Effects
{
    public class BiomeEffect : PermanentEffect
    {
        public BiomeEffect(Func<PlayerData, PlayerStat[]> apply, Func<PlayerData, PlayerStat[]> remove) : base(apply, remove) { }
    }
}
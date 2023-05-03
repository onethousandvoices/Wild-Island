using System;
using WildIsland.Data;

namespace Effects
{
    public class PermanentEffect : BaseEffect
    {
        public PermanentEffect(Func<PlayerData, PlayerStat[]> apply, Func<PlayerData, PlayerStat[]> remove) : base(apply, remove) { }
    }
}
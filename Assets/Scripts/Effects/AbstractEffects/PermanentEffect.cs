using System;
using WildIsland.Data;

namespace Effects
{
    public abstract class PermanentEffect : BaseEffect
    {
        protected PermanentEffect(Func<PlayerData, PlayerStat[]> apply, Func<PlayerData, PlayerStat[]> remove) : base(apply, remove) { }

        public override bool Process()
            => !IsExecuted;

        public override PlayerStat[] Apply(PlayerData data)
        {
            IsExecuted = true;
            return base.Apply(data);
        }
    }
}
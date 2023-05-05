using System;
using WildIsland.Data;

namespace Effects
{
    public abstract class PermanentEffect : BaseEffect
    {
        protected PermanentEffect(Action<PlayerData> apply = null) : base(apply) { }

        public override bool Process()
            => InstantApply();
    }
}
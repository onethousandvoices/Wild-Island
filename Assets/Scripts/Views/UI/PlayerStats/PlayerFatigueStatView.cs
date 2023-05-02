using System;
using WildIsland.Data;

namespace WildIsland.Views.UI
{
    public class PlayerFatigueStatView : BasePlayerStatView
    {
        public override Type TargetStat => typeof(PlayerFatigue);
    }
}
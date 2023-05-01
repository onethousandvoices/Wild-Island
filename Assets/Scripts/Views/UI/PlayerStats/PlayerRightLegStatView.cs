using System;
using WildIsland.Data;

namespace WildIsland.Views.UI
{
    public class PlayerRightLegStatView : BasePlayerStatView
    {
        public override Type TargetStat => typeof(PlayerRightLegHealth);
    }
}
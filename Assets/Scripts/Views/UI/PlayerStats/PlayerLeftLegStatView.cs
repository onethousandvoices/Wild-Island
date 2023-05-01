using System;
using WildIsland.Data;

namespace WildIsland.Views.UI
{
    public class PlayerLeftLegStatView : BasePlayerStatView
    {
        public override Type TargetStat => typeof(PlayerLeftLegHealth);
    }
}
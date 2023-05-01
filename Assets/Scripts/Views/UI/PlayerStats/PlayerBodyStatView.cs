using System;
using WildIsland.Data;

namespace WildIsland.Views.UI
{
    public class PlayerBodyStatView : BasePlayerStatView
    {
        public override Type TargetStat => typeof(PlayerBodyHealth);
    }
}
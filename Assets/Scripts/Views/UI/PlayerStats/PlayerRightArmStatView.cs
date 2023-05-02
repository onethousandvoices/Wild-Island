using System;
using WildIsland.Data;

namespace WildIsland.Views.UI
{
    public class PlayerRightArmStatView : BasePlayerStatView
    {
        public override Type TargetStat => typeof(PlayerRightArmHealth);
    }
}
using System;
using WildIsland.Data;

namespace WildIsland.Views.UI
{
    public class PlayerLeftArmStatView : BasePlayerStatView
    {
        public override Type TargetStat => typeof(PlayerLeftArmHealth);
    }
}
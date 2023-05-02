using System;
using WildIsland.Data;

namespace WildIsland.Views.UI
{
    public class PlayerStaminaStatView : BasePlayerStatView
    {
        public override Type TargetStat => typeof(PlayerStamina);
    }
}
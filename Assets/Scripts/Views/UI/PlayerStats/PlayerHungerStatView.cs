using System;
using WildIsland.Data;

namespace WildIsland.Views.UI
{
    public class PlayerHungerStatView : BasePlayerStatView
    {
        public override Type TargetStat => typeof(PlayerHunger);
    }
}
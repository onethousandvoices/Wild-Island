using System;
using WildIsland.Data;

namespace WildIsland.Views.UI
{
    public class PlayerThirstStatView : BasePlayerStatView
    {
        public override Type TargetStat => typeof(PlayerThirst);
    }
}
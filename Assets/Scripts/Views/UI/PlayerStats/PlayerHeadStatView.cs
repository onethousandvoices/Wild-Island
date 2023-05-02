using System;
using WildIsland.Data;

namespace WildIsland.Views.UI
{
    public class PlayerHeadStatView : BasePlayerStatView
    {
        public override Type TargetStat => typeof(PlayerHeadHealth);
    }
}
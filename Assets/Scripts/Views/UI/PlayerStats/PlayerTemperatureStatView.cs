using System;
using WildIsland.Data;

namespace WildIsland.Views.UI
{
    public class PlayerTemperatureStatView : BasePlayerStatView
    {
        public override Type TargetStat => typeof(PlayerTemperature);
    }
}
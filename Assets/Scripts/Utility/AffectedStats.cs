using System;
using System.Collections.Generic;
using System.Linq;
using WildIsland.Data;

namespace WildIsland.Utility
{
    public class AffectedStats : List<Tuple<PlayerStat, float>>
    {
        public PlayerStat[] ApplyReturnStats
        {
            get
            {
                foreach (Tuple<PlayerStat, float> pair in this)
                    pair.Item1.Value += pair.Item2;
                return this.Select(x => x.Item1).ToArray();
            }
        }

        public PlayerStat[] RevertReturnStats
        {
            get
            {
                foreach (Tuple<PlayerStat, float> pair in this)
                {
                    if (pair.Item1 is not VolatilePlayerStat) 
                        continue;
                    pair.Item1.Value -= pair.Item2;
                }
                return this.Select(x => x.Item1).ToArray();
            }
        }
    }
}
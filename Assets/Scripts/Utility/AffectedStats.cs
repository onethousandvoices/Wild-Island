using System.Collections.Generic;
using System.Linq;
using WildIsland.Data;

namespace WildIsland.Utility
{
    public class AffectedStats : List<AffectedStat>
    {
        private PlayerStat[] ReturnArray => this.Select(x => x.Stat).ToArray();

        public PlayerStat[] ApplyReturnStats
        {
            get
            {
                foreach (AffectedStat pair in this)
                    pair.Stat.ApplyValue(pair.Value);
                return ReturnArray;
            }
        }

        public void RevertClear()
        {
            foreach (AffectedStat pair in this)
            {
                if (pair.Stat is not VolatilePlayerStat)
                    continue;
                pair.Stat.ApplyValue(-pair.Value);
            }

            Clear();
        }
    }

    public class AffectedStat
    {
        public readonly PlayerStat Stat;
        public readonly float Value;

        public AffectedStat(PlayerStat stat, float value)
        {
            Stat = stat;
            Value = value;
        }
    }
}
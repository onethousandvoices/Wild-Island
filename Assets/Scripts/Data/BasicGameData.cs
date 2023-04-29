using System;
using System.Collections.Generic;
using WildIsland.Data;

namespace Data
{
    [Serializable]
    public class BasicGameData : IGDDDataTypeString
    {
        [Serializable]
        public class DaySettings
        {
            [SheetColumnName("A1")]
            public readonly int DayTimer = 0;

            [SheetColumnName("A2")]
            public readonly int NightTimer = 0;
        }
        
        public readonly DaySettings DaySettingsData = null;

        public string ID => "Basic";
        
        public static Dictionary<string, Type> RowTypes = new Dictionary<string, Type>
        {
            { "Day", typeof(DaySettings)}
        };
    }
    
}
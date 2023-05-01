using System;
using System.Collections.Generic;

namespace WildIsland.Data
{
    [Serializable]
    public class BasicGameData : IGDDDataTypeString
    {
        public string ID => "Basic";

        [Serializable]
        public class DaySettings
        {
            [SheetColumnName("A1")]
            public readonly int DayTimer = 0;

            [SheetColumnName("A2")]
            public readonly int NightTimer = 0;

            [SheetColumnName("A3")]
            public readonly int DayTemperatureAffectStage1 = 0;
            
            [SheetColumnName("A3")]
            public readonly int DayTemperatureAffectStage2 = 0;
            
            [SheetColumnName("A3")]
            public readonly int DayTemperatureAffectStage3 = 0;
            
            [SheetColumnName("A3")]
            public readonly int NightTemperatureAffectStage1 = 0;
            
            [SheetColumnName("A3")]
            public readonly int NightTemperatureAffectStage2 = 0;
        }
        
        public readonly DaySettings DaySettingsData = null;

        public static Dictionary<string, Type> RowTypes = new Dictionary<string, Type>
        {
            { "Day", typeof(DaySettings)}
        };
    }
}
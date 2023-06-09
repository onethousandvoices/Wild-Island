using System;

namespace WildIsland.Data
{
    [Serializable]
    public class BasicGameData : IGameData
    {
        public DaySettings DaySettingsData;

        [Serializable]
        public class DaySettings
        {
            [Sheet("A1")] public int DayTimer;
            [Sheet("A2")] public int NightTimer;
            [Sheet("A3")] public int DayTemperatureAffectStage1;
            [Sheet("A4")] public int DayTemperatureAffectStage2;
            [Sheet("A5")] public int DayTemperatureAffectStage3;
            [Sheet("A6")] public int NightTemperatureAffectStage1;
            [Sheet("A7")] public int NightTemperatureAffectStage2;
        }
    }
}
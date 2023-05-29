using System;

namespace WildIsland.Data
{
    [Serializable]
    public class BiomesData : IGameData<BiomeData>
    {
        public BiomeData Forest => Datas["forest"];
        public BiomeData Winter => Datas["winter"];
        public BiomeData Desert => Datas["desert"];
        public BiomeData Swamp => Datas["swamp"];
    }

    [Serializable]
    public class BiomeData
    {
        [Sheet("BiomeTemperature")] public float Temperature;
        [Sheet("EffectValue")] public float EffectValue;
    }
}
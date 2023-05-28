using System;

namespace WildIsland.Data
{
    [Serializable]
    public class BiomesData : IGameData
    {
        public ForestBiomeData ForestBiomeData;
        public WinterBiomeData WinterBiomeData;
        public DesertBiomeData DesertBiomeData;
        public SwampBiomeData SwampBiomeData;
    }
    
    [Serializable]
    public class BiomeData
    {
        [Sheet("BiomeTemperature")] public float Temperature;
        [Sheet("EffectValue")] public float EffectValue;
    }
    
    [Serializable]
    public class ForestBiomeData : BiomeData { }

    [Serializable]
    public class WinterBiomeData : BiomeData { }

    [Serializable]
    public class DesertBiomeData : BiomeData { }

    [Serializable]
    public class SwampBiomeData : BiomeData { }
}
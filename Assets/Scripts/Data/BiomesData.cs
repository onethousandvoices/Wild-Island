using System;

namespace WildIsland.Data
{
    [Serializable]
    public class BiomesData : IGDDDataTypeString
    {
        public string ID => "Biomes";

        public ForestBiomeData ForestBiomeData;
        public WinterBiomeData WinterBiomeData;
        public DesertBiomeData DesertBiomeData;
        public SwampBiomeData SwampBiomeData;
    }

    [Serializable]
    public class ForestBiomeData : BiomeData { }

    [Serializable]
    public class WinterBiomeData : BiomeData { }

    [Serializable]
    public class DesertBiomeData : BiomeData { }

    [Serializable]
    public class SwampBiomeData : BiomeData { }

    [Serializable]
    public class BiomeData
    {
        public float Temperature;
        public float EffectValue;
    }
}
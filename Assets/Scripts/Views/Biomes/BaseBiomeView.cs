using UnityEngine;
using WildIsland.Data;

namespace Views.Biomes
{
    public class BaseBiomeView : MonoBehaviour 
    {
        public BiomeData Data { get; private set; }

        public void SetData(BiomeData data)
            => Data = data;
    }
}
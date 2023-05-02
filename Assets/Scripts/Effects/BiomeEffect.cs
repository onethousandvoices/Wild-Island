using WildIsland.Data;

namespace Effects
{
    public class BiomeEffect : PeriodicEffect
    {
        public readonly BiomeData Data;

        public BiomeEffect(BiomeData data, params PlayerDataEffect[] effects) : base(1f, effects)
            => Data = data;
    }
}
using Views.Biomes;
using WildIsland.Data;
using Zenject;

namespace WildIsland.Controllers
{
    public class BiomeController : IInitializable, IBiomeDayAffect
    {
        [Inject] private BiomesData _biomesData;
        [InjectOptional] private ForestBiomeView _forest;
        [InjectOptional] private WinterBiomeView _winter;
        [InjectOptional] private DesertBiomeView _desert;
        [InjectOptional] private SwampBiomeView _swamp;

        private BaseBiomeView[] _biomes;

        public void Initialize()
        {
            if (_forest != null)
                _forest.Init(_biomesData.ForestBiomeData);
            if (_winter != null)
                _winter.Init(_biomesData.WinterBiomeData);
            if (_desert != null)
                _desert.Init(_biomesData.DesertBiomeData);
            if (_swamp != null)
                _swamp.Init(_biomesData.SwampBiomeData);

            _biomes = new BaseBiomeView[]
            {
                _forest,
                _winter,
                _desert,
                _swamp
            };
        }

        public void UpdateBiomesTemperature(float temperatureAffect)
        {
            foreach (BaseBiomeView biome in _biomes)
            {
                if (biome == null)
                    continue;
                biome.UpdateTemperature(temperatureAffect);
            }
        }
    }

    public interface IBiomeDayAffect
    {
        public void UpdateBiomesTemperature(float temperatureAffect);
    }
}
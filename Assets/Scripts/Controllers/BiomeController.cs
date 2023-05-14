using System;
using Views.Biomes;
using WildIsland.Data;
using Zenject;

namespace WildIsland.Controllers
{
    public class BiomeController : IInitializable, IGDConsumer, IBiomeDayAffect
    {
        [InjectOptional] private ForestBiomeView _forest;
        [InjectOptional] private WinterBiomeView _winter;
        [InjectOptional] private DesertBiomeView _desert;
        [InjectOptional] private SwampBiomeView _swamp;

        private BaseBiomeView[] _biomes;
        private BiomesData _data;

        public Type ContainerType => typeof(BiomesDataContainer);

        public void AcquireGameData(IPartialGameDataContainer container)
            => _data = ((BiomesDataContainer)container).Default;

        public void Initialize()
        {
            if (_forest != null)
                _forest.Init(_data.ForestBiomeData);
            if (_winter != null)
                _winter.Init(_data.WinterBiomeData);
            if (_desert != null)
                _desert.Init(_data.DesertBiomeData);
            if (_swamp != null)
                _swamp.Init(_data.SwampBiomeData);

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
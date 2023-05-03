using System;
using UnityEngine;
using Views.Biomes;
using WildIsland.Data;
using Zenject;

namespace WildIsland.Controllers
{
    public class BiomeController : IInitializable, IGDConsumer, IBiomeDayAffect
    {
        [Inject] private ForestBiomeView _forest;
        [Inject] private WinterBiomeView _winter;
        [Inject] private DesertBiomeView _desert;
        [Inject] private SwampBiomeView _swamp;

        private BaseBiomeView[] _biomes;
        private BiomesData _data;

        public Type ContainerType => typeof(BiomesDataContainer);
        public void AcquireGameData(IPartialGameDataContainer container)
            => _data = ((BiomesDataContainer)container).Default;

        public void Initialize()
        {
            _forest.Init(_data.ForestBiomeData);
            _winter.Init(_data.WinterBiomeData);
            _desert.Init(_data.DesertBiomeData);
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
            Debug.Log("biome temps updated");
            foreach (BaseBiomeView biome in _biomes)
                biome.UpdateTemperature(temperatureAffect);
        }
    }

    public interface IBiomeDayAffect
    {
        public void UpdateBiomesTemperature(float temperatureAffect);
    }
}
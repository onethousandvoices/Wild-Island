using System;
using Views.Biomes;
using WildIsland.Data;
using Zenject;

namespace WildIsland.Controllers
{
    public class BiomeController : IInitializable, IGDConsumer
    {
        [Inject] private ForestBiomeView _forest;
        [Inject] private WinterBiomeView _winter;
        [Inject] private DesertBiomeView _desert;
        [Inject] private SwampBiomeView _swamp;
        
        private BiomesData _data;

        public Type ContainerType => typeof(BiomesDataContainer);
        public void AcquireGameData(IPartialGameDataContainer container)
            => _data = ((BiomesDataContainer)container).Default;

        public void Initialize()
        {
            _forest.SetData(_data.ForestBiomeData);
            _winter.SetData(_data.WinterBiomeData);
            _desert.SetData(_data.DesertBiomeData);
            _swamp.SetData(_data.SwampBiomeData);
        }
    }
}
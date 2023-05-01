using System;
using Views.Biomes;
using WildIsland.Data;
using Zenject;

namespace WildIsland.Controllers
{
    public class BiomeController : IInitializable, ITickable, IGDConsumer
    {
        [Inject] private ForestBiomeView _forest;
        
        private BiomesData _data;

        public Type ContainerType => typeof(BiomesDataContainer);
        public void AcquireGameData(IPartialGameDataContainer container)
            => _data = ((BiomesDataContainer)container).Default;

        public void Initialize()
        {
            _forest.SetData(_data.ForestBiomeData);
        }

        public void Tick()
        {
        }
    }
}
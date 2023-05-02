using System;
using System.Collections.Generic;
using WildIsland.Data;
using Zenject;

namespace WildIsland.Controllers
{
    public class GameDataController : IInitializable
    {
        [Inject] private List<IGDConsumer> _consumers;
        private GameDataBase _gdInstance;

        public void Initialize()
        {
            _gdInstance = GameDataBase.Load<GameData>("gameData");
            SpreadGameDataInstance();
        }

        private void SpreadGameDataInstance()
        {
            Dictionary<Type, IPartialGameDataContainer> hashedContainers = new Dictionary<Type, IPartialGameDataContainer>();

            foreach (IGDConsumer consumer in _consumers)
            {
                if (!hashedContainers.ContainsKey(consumer.ContainerType))
                    hashedContainers[consumer.ContainerType] = _gdInstance.PrepareContainer(consumer.ContainerType);
                
                try
                {
                    consumer.AcquireGameData(hashedContainers[consumer.ContainerType]);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError(e);
                }
            }
        }
    }

    public interface IGDConsumer
    {
        public void AcquireGameData(IPartialGameDataContainer container);
        public Type ContainerType { get; }
    }
}
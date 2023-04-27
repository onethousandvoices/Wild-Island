using UnityEngine;
using Zenject;

namespace WildIsland.Test
{
    public class TestInstaller : MonoInstaller
    {
        [SerializeField] private Entity _entityPrefab;
        
        public override void InstallBindings()
        {
            Container.Bind<Injected>().AsSingle();
            Container.BindFactory<Entity, EntityFactory>().FromComponentInNewPrefab(_entityPrefab);
        }
    }
}
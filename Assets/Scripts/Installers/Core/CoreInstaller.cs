using UnityEngine;
using WildIsland.Controllers;
using WildIsland.Data;
using WildIsland.Processors;
using Zenject;

namespace WildIsland.Installers
{
    public class CoreInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Gdd gdd = Resources.Load<Gdd>("gdd");

            Container.Bind<BasicGameData>().FromInstance(gdd.BasicGameData);
            Container.Bind<BiomesData>().FromInstance(gdd.BiomesData);
            Container.Bind<PlayerData>().FromInstance(gdd.PlayerData);
            
            Container.BindInterfacesTo<GameController>().AsSingle();
            Container.BindInterfacesTo<TimeTickablesController>().AsSingle();
            
            Container.BindInterfacesTo<DayController>().AsSingle();
            Container.BindInterfacesTo<BiomeController>().AsSingle();
            Container.BindInterfacesTo<DebugConsoleController>().AsSingle();

            Container.BindInterfacesTo<PlayerDataProcessor>().AsSingle();
            Container.BindInterfacesTo<PlayerEffectProcessor>().AsSingle();
            Container.BindInterfacesTo<PlayerSoundProcessor>().AsSingle();
            Container.BindInterfacesTo<PlayerProcessor>().AsSingle();
            Container.BindInterfacesTo<PlayerInventoryProcessor>().AsSingle();
            Container.BindInterfacesTo<PlayerCameraProcessor>().AsSingle();
            Container.BindInterfacesTo<PlayerItemProcessor>().AsSingle();
            Container.BindInterfacesTo<PlayerController>().AsSingle();
        }
    }
}
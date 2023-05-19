using Core;
using WildIsland.Controllers;
using WildIsland.Processors;
using Zenject;

namespace WildIsland.Installers
{
    public class CoreInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GameDataController>().AsSingle();
            Container.BindInterfacesTo<DayController>().AsSingle();
            Container.BindInterfacesTo<BiomeController>().AsSingle();
            Container.BindInterfacesTo<DebugConsoleController>().AsSingle();

            Container.Bind<PlayerInput>().AsSingle();
            Container.BindInterfacesTo<PlayerDataProcessor>().AsSingle();
            Container.BindInterfacesTo<PlayerEffectProcessor>().AsSingle();
            Container.BindInterfacesTo<PlayerSoundProcessor>().AsSingle();
            Container.BindInterfacesTo<PlayerInputProcessor>().AsSingle();
            Container.BindInterfacesTo<PlayerInventoryProcessor>().AsSingle();
            Container.BindInterfacesTo<PlayerController>().AsSingle();
        }
    }
}
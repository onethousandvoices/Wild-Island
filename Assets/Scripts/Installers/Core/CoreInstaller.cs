using WildIsland.Controllers;
using WildIsland.Processors;
using Zenject;

namespace WildIsland.Installers
{
    public class CoreInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // Container.BindInitializableExecutionOrder<GameDataController>(-100);
            Container.BindInterfacesTo<GameDataController>().AsSingle();
            Container.BindInterfacesTo<DayController>().AsSingle();
            Container.BindInterfacesTo<BiomeController>().AsSingle();
            
            // Container.BindInitializableExecutionOrder<PlayerDataProcessor>(-50);
            Container.BindInterfacesTo<PlayerDataProcessor>().AsSingle();
            Container.BindInterfacesTo<PlayerEffectProcessor>().AsSingle();
            Container.BindInterfacesTo<PlayerSoundProcessor>().AsSingle();
            Container.BindInterfacesTo<PlayerInputProcessor>().AsSingle();
            Container.BindInterfacesTo<PlayerCheatProcessor>().AsSingle();
            Container.BindInterfacesTo<PlayerController>().AsSingle();
        }
    }
}
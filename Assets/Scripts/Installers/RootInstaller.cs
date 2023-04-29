using WildIsland.Controllers;
using Zenject;

namespace WildIsland.Installers
{
    public class RootInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GameDataController>().AsSingle();
            Container.BindInterfacesTo<DayController>().AsSingle();
            Container.BindInterfacesTo<PlayerController>().AsSingle();
        }
    }
}
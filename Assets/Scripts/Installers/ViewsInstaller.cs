using UnityEngine;
using WildIsland.Views;
using Zenject;

namespace WildIsland.Installers
{
    public class ViewsInstaller : MonoInstaller
    {
        [SerializeField] private DayLightView _dayLightView;
        [SerializeField] private PlayerView _playerView;
        [SerializeField] private Camera _camera;
        
        public override void InstallBindings()
        {
            Container.Bind<DayLightView>().FromInstance(_dayLightView).AsSingle();
            Container.Bind<PlayerView>().FromInstance(_playerView).AsSingle();
            Container.Bind<Camera>().FromInstance(_camera).AsSingle();
        }
    }
}
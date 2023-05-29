using UnityEngine;
using Views.Biomes;
using WildIsland.Views;
using Zenject;

namespace WildIsland.Installers
{
    public class ViewsInstaller : MonoInstaller
    {
        [SerializeField] private DayLightView _dayLightView;
        [SerializeField] private PlayerView _playerView;
        [SerializeField] private PlayerCamerasView _playerCamerasView;
        [SerializeField] private Camera _camera;
        [SerializeField] private BaseBiomeView[] _biomeViews;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<DayLightView>().FromInstance(_dayLightView).AsSingle();
            Container.Bind<PlayerView>().FromInstance(_playerView).AsSingle();
            Container.Bind<PlayerCamerasView>().FromInstance(_playerCamerasView).AsSingle();
            Container.Bind<Camera>().FromInstance(_camera).AsSingle();

            foreach (BaseBiomeView view in _biomeViews)
            {
                if (view == null)
                    continue;
                Container.Bind(view.GetType()).FromInstance(view).AsSingle();
            }
        }
    }
}
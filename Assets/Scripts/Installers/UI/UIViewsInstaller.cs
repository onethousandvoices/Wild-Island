using UnityEngine;
using Views.UI;
using WildIsland.Views.UI;
using Zenject;

namespace WildIsland.Installers
{
    public class UIViewsInstaller : MonoInstaller
    {
        [SerializeField] private PlayerViewStatsHolder _playerViewStatsHolder;
        [SerializeField] private DayTimerView _dayTimerView;
        
        public override void InstallBindings()
        {
            Container.Bind<PlayerViewStatsHolder>().FromInstance(_playerViewStatsHolder);
            Container.Bind<DayTimerView>().FromInstance(_dayTimerView);
        }
    }
}
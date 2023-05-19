using UnityEngine;
using Views.UI;
using Views.UI.Inventory;
using WildIsland.Views.UI;
using Zenject;

namespace WildIsland.Installers
{
    public class UIViewsInstaller : MonoInstaller
    {
        [SerializeField] private PlayerViewStatsHolder _playerViewStatsHolder;
        [SerializeField] private DayTimerView _dayTimerView;
        [SerializeField] private InventoryView _inventoryView;
        [SerializeField] private DebugConsoleView _debugConsoleView;
        [SerializeField] private InventoryItemViewBase _inventoryItemViewBasePrefab;

        public override void InstallBindings()
        {
            Container.Bind<PlayerViewStatsHolder>().FromInstance(_playerViewStatsHolder);
            Container.Bind<DayTimerView>().FromInstance(_dayTimerView);
            Container.Bind<InventoryView>().FromInstance(_inventoryView);
            Container.BindInterfacesAndSelfTo<DebugConsoleView>().FromInstance(_debugConsoleView);
            Container.Bind<InventoryItemViewBase>().FromInstance(_inventoryItemViewBasePrefab);
        }
    }
}
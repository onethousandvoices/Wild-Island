﻿using UnityEngine;
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

        
        public override void InstallBindings()
        {
            Container.Bind<PlayerViewStatsHolder>().FromInstance(_playerViewStatsHolder);
            Container.Bind<DayTimerView>().FromInstance(_dayTimerView);
            Container.Bind<InventoryView>().FromInstance(_inventoryView).AsSingle();
        }
    }
}
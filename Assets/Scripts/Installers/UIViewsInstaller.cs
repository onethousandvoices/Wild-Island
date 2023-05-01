using UnityEngine;
using WildIsland.Views.UI;
using Zenject;

namespace WildIsland.Installers
{
    public class UIViewsInstaller : MonoInstaller
    {
        [SerializeField] private PlayerViewStatsHolder playerViewStatsHolder;
        
        public override void InstallBindings()
        {
            Container.Bind<PlayerViewStatsHolder>().FromInstance(playerViewStatsHolder);
        }
    }
}
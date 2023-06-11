using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WildIsland.Data;
using WildIsland.Processors;
using WildIsland.Views.UI;
using Zenject;

namespace WildIsland.Controllers
{
    public class PlayerController : IInitializable, ITickable, IPlayerStatSetter
    {
        [Inject] private PlayerViewStatsHolder _viewStatsHolder;
        [Inject] private IGetPlayerStats _player;
        [Inject] private List<IPlayerProcessor> _playerProcessors;

        private Dictionary<PlayerStat, BasePlayerStatView> _statViewPairs;
        private List<BasePlayerStatView> _basePlayerStatViews;

        public void Initialize()
        {
            _statViewPairs = new Dictionary<PlayerStat, BasePlayerStatView>
            {
                { _player.Stats.HeadHealth, _viewStatsHolder.PlayerHeadStatView },
                { _player.Stats.BodyHealth, _viewStatsHolder.PlayerBodyStatView },
                { _player.Stats.LeftArmHealth, _viewStatsHolder.PlayerLeftArmStatView },
                { _player.Stats.RightArmHealth, _viewStatsHolder.PlayerRightArmStatView },
                { _player.Stats.LeftLegHealth, _viewStatsHolder.PlayerLeftLegStatView },
                { _player.Stats.RightLegHealth, _viewStatsHolder.PlayerRightLegStatView },
                { _player.Stats.Stamina, _viewStatsHolder.PlayerStaminaStatView },
                { _player.Stats.Hunger, _viewStatsHolder.PlayerHungerStatView },
                { _player.Stats.Thirst, _viewStatsHolder.PlayerThirstStatView },
                { _player.Stats.Fatigue, _viewStatsHolder.PlayerFatigueStatView },
            };

            _viewStatsHolder.PlayerBodyStatView.SetRefs(_player.Stats.BodyHealth);
            _viewStatsHolder.PlayerHeadStatView.SetRefs(_player.Stats.HeadHealth);
            _viewStatsHolder.PlayerLeftArmStatView.SetRefs(_player.Stats.LeftArmHealth);
            _viewStatsHolder.PlayerRightArmStatView.SetRefs(_player.Stats.RightArmHealth);
            _viewStatsHolder.PlayerLeftLegStatView.SetRefs(_player.Stats.LeftLegHealth);
            _viewStatsHolder.PlayerRightLegStatView.SetRefs(_player.Stats.RightLegHealth);
            _viewStatsHolder.PlayerStaminaStatView.SetRefs(_player.Stats.Stamina);
            _viewStatsHolder.PlayerHungerStatView.SetRefs(_player.Stats.Hunger);
            _viewStatsHolder.PlayerThirstStatView.SetRefs(_player.Stats.Thirst);
            _viewStatsHolder.PlayerFatigueStatView.SetRefs(_player.Stats.Fatigue);
            
            _basePlayerStatViews = _statViewPairs.Values.ToList();
            _basePlayerStatViews.ForEach(x => x.Init());
            
            _playerProcessors.ForEach(x => x.Enable());

            QualitySettings.vSyncCount = 0;
        }

        public void Tick()
            => _basePlayerStatViews.ForEach(x => x.UpdateValue());

        public void SetStat(PlayerStat stat, float value = 0, bool forceDebugShow = false)
        {
            stat.ApplyValue(value);
            _statViewPairs.TryGetValue(stat, out BasePlayerStatView statView);
            if (statView == null)
                return;
            float perSecondsMult = Mathf.Abs(value) < 1 ? Time.deltaTime : 1;
            statView.UpdateDebugValue(value / perSecondsMult, forceDebugShow);
        }
    }

    public interface IPlayerStatSetter
    {
        public void SetStat(PlayerStat stat, float value = 0, bool forceDebugShow = false);
    }
}
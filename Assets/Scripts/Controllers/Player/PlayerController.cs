using System.Collections.Generic;
using UnityEngine;
using WildIsland.Data;
using WildIsland.Views.UI;
using Zenject;

namespace WildIsland.Controllers
{
    public class PlayerController : IInitializable, ITickable, IPlayerStatSetter
    {
        [Inject] private PlayerViewStatsHolder _viewStatsHolder;
        [Inject] private IGetPlayerStats _playerStats;
        [Inject] private List<ITickableProcessor> _tickableProcessors;

        private Dictionary<PlayerStat, BasePlayerStatView> _statViewPairs;

        public void Initialize()
        {
            _statViewPairs = new Dictionary<PlayerStat, BasePlayerStatView>
            {
                { _playerStats.Stats.HeadHealth, _viewStatsHolder.PlayerHeadStatView },
                { _playerStats.Stats.BodyHealth, _viewStatsHolder.PlayerBodyStatView },
                { _playerStats.Stats.LeftArmHealth, _viewStatsHolder.PlayerLeftArmStatView },
                { _playerStats.Stats.RightArmHealth, _viewStatsHolder.PlayerRightArmStatView },
                { _playerStats.Stats.LeftLegHealth, _viewStatsHolder.PlayerLeftLegStatView },
                { _playerStats.Stats.RightLegHealth, _viewStatsHolder.PlayerRightLegStatView },
                { _playerStats.Stats.Stamina, _viewStatsHolder.PlayerStaminaStatView },
                { _playerStats.Stats.Hunger, _viewStatsHolder.PlayerHungerStatView },
                { _playerStats.Stats.Thirst, _viewStatsHolder.PlayerThirstStatView },
                { _playerStats.Stats.Fatigue, _viewStatsHolder.PlayerFatigueStatView },
            };

            _viewStatsHolder.PlayerBodyStatView.SetRefs(_playerStats.Stats.BodyHealth, _playerStats.Default.BodyHealth);
            _viewStatsHolder.PlayerHeadStatView.SetRefs(_playerStats.Stats.HeadHealth, _playerStats.Default.HeadHealth);
            _viewStatsHolder.PlayerLeftArmStatView.SetRefs(_playerStats.Stats.LeftLegHealth, _playerStats.Default.LeftLegHealth);
            _viewStatsHolder.PlayerRightArmStatView.SetRefs(_playerStats.Stats.RightArmHealth, _playerStats.Default.RightArmHealth);
            _viewStatsHolder.PlayerLeftLegStatView.SetRefs(_playerStats.Stats.LeftLegHealth, _playerStats.Default.LeftLegHealth);
            _viewStatsHolder.PlayerRightLegStatView.SetRefs(_playerStats.Stats.RightLegHealth, _playerStats.Default.RightLegHealth);
            _viewStatsHolder.PlayerStaminaStatView.SetRefs(_playerStats.Stats.Stamina, _playerStats.Default.Stamina);
            _viewStatsHolder.PlayerHungerStatView.SetRefs(_playerStats.Stats.Hunger, _playerStats.Default.Hunger);
            _viewStatsHolder.PlayerThirstStatView.SetRefs(_playerStats.Stats.Thirst, _playerStats.Default.Thirst);
            _viewStatsHolder.PlayerFatigueStatView.SetRefs(_playerStats.Stats.Fatigue, _playerStats.Default.Fatigue);
        }

        public void Tick()
        {
            foreach (ITickableProcessor processor in _tickableProcessors)
                processor.Tick();
        }

        public void SetStat(PlayerStat stat, float value = 0, bool forceDebugShow = false)
        {
            stat.Value += value;
            if (stat.Value < 0)
                stat.Value = 0;
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

    public interface IGetPlayerStats
    {
        public PlayerData Stats { get; }
        public PlayerData Default { get; }
    }
}
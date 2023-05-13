using System;
using UnityEngine;
using WildIsland.Controllers;
using WildIsland.Data;
using WildIsland.Views;
using Zenject;
using Random = UnityEngine.Random;

namespace WildIsland.Processors
{
    public class PlayerDataProcessor : BaseProcessor, IInitializable, IPlayerProcessor, IDataProcessor, IDisposable, IGetPlayerStats, IGDConsumer
    {
        [Inject] private PlayerView _view;
        [Inject] private IPlayerInputState _inputState;
        [Inject] private IPlayerStatSetter _statSetter;
        [Inject] private IPlayerSpeed _playerSpeed;
        [Inject] private IGetPlayerStats _player;

        private DbValue<PlayerData> _data;
        private PlayerData _dataContainer;
        private float _relativeSpeed;

        public PlayerData Stats => _data.Value;

        public Type ContainerType => typeof(PlayerDataContainer);

        public void AcquireGameData(IPartialGameDataContainer container)
            => _dataContainer = ((PlayerDataContainer)container).Default;

        public void Initialize()
        {
            _data = new DbValue<PlayerData>("PlayerData", _dataContainer);
            Debug.Log(_data.Value.RegularSpeed.Value);
            Stats.SetDefaults();
        }

        public void Tick()
        {
            if (!Enabled)
                return;

            _relativeSpeed = _playerSpeed.CurrentSpeed / _player.Stats.SprintSpeed.Value;

            ProcessHealth();
            ProcessStamina();
            ProcessHunger();
            ProcessFatigue();
            ProcessThirst();
        }

        private void ProcessHealth()
        {
            float hungerMod = 0f;
            float thirstMod = 0f;
            float currentHunger = _player.Stats.Hunger.Value;
            float currentThirst = _player.Stats.Thirst.Value;

            if (currentHunger <= _view.HungerRegenStage1Range.y && currentHunger >= _view.HungerRegenStage1Range.x)
                hungerMod = _player.Stats.HealthRegenHungerStage1.Default;
            else if (currentHunger <= _view.HungerRegenStage2Range.y && currentHunger >= _view.HungerRegenStage2Range.x)
                hungerMod = _player.Stats.HealthRegenHungerStage2.Default;
            else if (currentHunger <= _view.HungerRegenStage3Range.y && currentHunger >= _view.HungerRegenStage3Range.x)
                hungerMod = _player.Stats.HealthRegenHungerStage3.Default;
            else if (currentHunger <= _view.HungerRegenStage4Range.y)
                hungerMod = _player.Stats.HealthRegenHungerStage4.Default;

            if (currentThirst <= _view.ThirstRegenStage1Range.y && currentThirst >= _view.ThirstRegenStage1Range.x)
                thirstMod = _player.Stats.HealthRegenThirstStage1.Default;
            else if (currentThirst <= _view.ThirstRegenStage2Range.y && currentThirst >= _view.ThirstRegenStage2Range.x)
                thirstMod = _player.Stats.HealthRegenThirstStage2.Default;
            else if (currentThirst <= _view.ThirstRegenStage3Range.y && currentThirst >= _view.ThirstRegenStage3Range.x)
                thirstMod = _player.Stats.HealthRegenThirstStage3.Default;
            else if (currentThirst <= _view.ThirstRegenStage4Range.y)
                thirstMod = _player.Stats.HealthRegenThirstStage4.Default;

            float currentRegen = _player.Stats.HealthRegen.Value + hungerMod + thirstMod;
            SetAllHealths(currentRegen * Time.deltaTime);
        }

        public void SetAllHealths(float value = 0f, bool isRandomizing = false)
        {
            bool decreasing = value < 0;

            if (_player.Stats.HeadHealth.Value < _player.Stats.HeadHealth.Default || decreasing || isRandomizing)
                _statSetter.SetStat(_player.Stats.HeadHealth, value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
            if (_player.Stats.BodyHealth.Value < _player.Stats.BodyHealth.Default || decreasing || isRandomizing)
                _statSetter.SetStat(_player.Stats.BodyHealth, value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
            if (_player.Stats.LeftArmHealth.Value < _player.Stats.LeftArmHealth.Default || decreasing || isRandomizing)
                _statSetter.SetStat(_player.Stats.LeftArmHealth, value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
            if (_player.Stats.RightArmHealth.Value < _player.Stats.RightArmHealth.Default || decreasing || isRandomizing)
                _statSetter.SetStat(_player.Stats.RightArmHealth, value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
            if (_player.Stats.LeftLegHealth.Value < _player.Stats.LeftLegHealth.Default || decreasing || isRandomizing)
                _statSetter.SetStat(_player.Stats.LeftLegHealth, value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
            if (_player.Stats.RightLegHealth.Value < _player.Stats.RightLegHealth.Default || decreasing || isRandomizing)
                _statSetter.SetStat(_player.Stats.RightLegHealth, value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
        }

        private void ProcessStamina()
        {
            if (Math.Abs(_player.Stats.Stamina.Value - _player.Stats.Stamina.Default) < 0.01f ||
                _inputState.State == PlayerInputState.Jump || _inputState.State == PlayerInputState.Sprint)
                return;

            float currentFatigue = _player.Stats.Fatigue.Value / _player.Stats.Fatigue.Default;
            float currentHunger = _player.Stats.Hunger.Value / _player.Stats.Hunger.Default;
            float currentThirst = _player.Stats.Thirst.Value / _player.Stats.Thirst.Default;

            float currentRegen =
                _player.Stats.StaminaRegen.Value * (1 - _player.Stats.Stamina.Value / _player.Stats.Stamina.Default) * currentFatigue * currentHunger * currentThirst;

            _statSetter.SetStat(_player.Stats.Stamina, currentRegen * Time.deltaTime);
        }

        private void ProcessHunger()
        {
            if (_player.Stats.Hunger.Value <= 0)
                return;

            float currentHungerDecrease = _player.Stats.HungerDecrease.Value + _relativeSpeed;
            _statSetter.SetStat(_player.Stats.Hunger, -currentHungerDecrease * Time.deltaTime, true);
        }

        private void ProcessFatigue()
        {
            if (_player.Stats.Fatigue.Value <= 0)
                return;

            float currentFatigueDecrease = _player.Stats.FatigueDecrease.Value + _relativeSpeed;
            _statSetter.SetStat(_player.Stats.Fatigue, -currentFatigueDecrease * Time.deltaTime, true);
        }

        private void ProcessThirst()
        {
            if (_player.Stats.Thirst.Value <= 0)
                return;

            float currentThirstDecrease = _player.Stats.ThirstDecrease.Value + _relativeSpeed;
            _statSetter.SetStat(_player.Stats.Thirst, -currentThirstDecrease * Time.deltaTime, true);
        }

        public void Dispose()
            => _data.Save();
    }

    public interface IDataProcessor
    {
        public void SetAllHealths(float value = 0f, bool isRandomizing = false);
    }
    
    public interface IGetPlayerStats
    {
        public PlayerData Stats { get; }
    }
}
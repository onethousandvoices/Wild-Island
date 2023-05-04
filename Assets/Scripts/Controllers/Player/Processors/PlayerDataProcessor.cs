using System;
using UnityEngine;
using WildIsland.Data;
using WildIsland.Views;
using Zenject;
using Random = UnityEngine.Random;

namespace WildIsland.Controllers
{
    public class PlayerDataProcessor : BaseProcessor, IDataProcessor, IInitializable, ITickableProcessor, IDisposable, IGetPlayerStats, IGDConsumer
    {
        [Inject] private PlayerView _view;
        [Inject] private IPlayerInputState _inputState;
        [Inject] private IPlayerStatSetter _statSetter;
        [Inject] private IPlayerSpeed _playerSpeed;
        [Inject] private IGetPlayerStats _player;

        private DbValue<PlayerData> _data;

        public PlayerData Stats => _data.Value;
        public PlayerData Default { get; private set; }

        private float _relativeSpeed;

        public Type ContainerType => typeof(PlayerDataContainer);

        public void AcquireGameData(IPartialGameDataContainer container)
            => Default = ((PlayerDataContainer)container).Default;

        public void Initialize()
            => _data = new DbValue<PlayerData>("PlayerData", Default);

        public void Tick()
        {
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
                hungerMod = _player.Default.HealthRegenHungerStage1.Value;
            else if (currentHunger <= _view.HungerRegenStage2Range.y && currentHunger >= _view.HungerRegenStage2Range.x)
                hungerMod = _player.Default.HealthRegenHungerStage2.Value;
            else if (currentHunger <= _view.HungerRegenStage3Range.y && currentHunger >= _view.HungerRegenStage3Range.x)
                hungerMod = _player.Default.HealthRegenHungerStage3.Value;
            else if (currentHunger <= _view.HungerRegenStage4Range.y)
                hungerMod = _player.Default.HealthRegenHungerStage4.Value;

            if (currentThirst <= _view.ThirstRegenStage1Range.y && currentThirst >= _view.ThirstRegenStage1Range.x)
                thirstMod = _player.Default.HealthRegenThirstStage1.Value;
            else if (currentThirst <= _view.ThirstRegenStage2Range.y && currentThirst >= _view.ThirstRegenStage2Range.x)
                thirstMod = _player.Default.HealthRegenThirstStage2.Value;
            else if (currentThirst <= _view.ThirstRegenStage3Range.y && currentThirst >= _view.ThirstRegenStage3Range.x)
                thirstMod = _player.Default.HealthRegenThirstStage3.Value;
            else if (currentThirst <= _view.ThirstRegenStage4Range.y)
                thirstMod = _player.Default.HealthRegenThirstStage4.Value;

            float currentRegen = _player.Stats.HealthRegen.Value + hungerMod + thirstMod;
            SetAllHealths(currentRegen * Time.deltaTime);
        }

        public void SetAllHealths(float value = 0f, bool isRandomizing = false)
        {
            bool decreasing = value < 0;

            if (_player.Stats.HeadHealth.Value < _player.Default.HeadHealth.Value || decreasing || isRandomizing)
                _statSetter.SetStat(_player.Stats.HeadHealth, value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
            if (_player.Stats.BodyHealth.Value < _player.Default.BodyHealth.Value || decreasing || isRandomizing)
                _statSetter.SetStat(_player.Stats.BodyHealth, value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
            if (_player.Stats.LeftArmHealth.Value < _player.Default.LeftArmHealth.Value || decreasing || isRandomizing)
                _statSetter.SetStat(_player.Stats.LeftArmHealth, value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
            if (_player.Stats.RightArmHealth.Value < _player.Default.RightArmHealth.Value || decreasing || isRandomizing)
                _statSetter.SetStat(_player.Stats.RightArmHealth, value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
            if (_player.Stats.LeftLegHealth.Value < _player.Default.LeftLegHealth.Value || decreasing || isRandomizing)
                _statSetter.SetStat(_player.Stats.LeftLegHealth, value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
            if (_player.Stats.RightLegHealth.Value < _player.Default.RightLegHealth.Value || decreasing || isRandomizing)
                _statSetter.SetStat(_player.Stats.RightLegHealth, value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
        }

        private void ProcessStamina()
        {
            if (Math.Abs(_player.Stats.Stamina.Value - _player.Default.Stamina.Value) < 0.01f ||
                _inputState.State == PlayerInputState.Jump || _inputState.State == PlayerInputState.Sprint)
                return;

            float currentFatigue = _player.Stats.Fatigue.Value / _player.Default.Fatigue.Value;
            float currentHunger = _player.Stats.Hunger.Value / _player.Default.Hunger.Value;
            float currentThirst = _player.Stats.Thirst.Value / _player.Default.Thirst.Value;

            float currentRegen =
                _player.Stats.StaminaRegen.Value * (1 - _player.Stats.Stamina.Value / _player.Default.Stamina.Value) * currentFatigue * currentHunger * currentThirst;

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
        {
            //todo remove default
            _data.Save(Default);
        }
    }

    public interface IDataProcessor
    {
        public void SetAllHealths(float value = 0f, bool isRandomizing = false);
    }
}
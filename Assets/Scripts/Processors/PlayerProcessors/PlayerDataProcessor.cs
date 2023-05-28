using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;
using WildIsland.Controllers;
using WildIsland.Data;
using WildIsland.Views;
using Zenject;
using Random = UnityEngine.Random;

namespace WildIsland.Processors
{
    public class PlayerDataProcessor : BaseProcessor, IInitializable, IPlayerProcessor, ITickable, IDataProcessor, IDisposable, IGetPlayerStats
    {
        [Inject] private PlayerData _dataContainer;
        [Inject] private PlayerView _view;
        [Inject] private IPlayerStatSetter _statSetter;
        [Inject] private IPlayerState _playerState;

        private DbValue<PlayerData> _data;
        private float _relativeSpeed;

        public PlayerData Stats => _data.Value;

        public void Initialize()
        {
            _data = new DbValue<PlayerData>("PlayerData", new PlayerData(_dataContainer));
            _data.Value.SetDefaults();
        }

        public void Tick()
        {
            if (!Enabled)
                return;

            _relativeSpeed = _playerState.CurrentSpeed / Stats.SprintSpeed.Value;

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
            float currentHunger = Stats.Hunger.Value;
            float currentThirst = Stats.Thirst.Value;

            if (currentHunger <= _view.HungerRegenStage1Range.y && currentHunger >= _view.HungerRegenStage1Range.x)
                hungerMod = Stats.HealthRegenHungerStage1.Default;
            else if (currentHunger <= _view.HungerRegenStage2Range.y && currentHunger >= _view.HungerRegenStage2Range.x)
                hungerMod = Stats.HealthRegenHungerStage2.Default;
            else if (currentHunger <= _view.HungerRegenStage3Range.y && currentHunger >= _view.HungerRegenStage3Range.x)
                hungerMod = Stats.HealthRegenHungerStage3.Default;
            else if (currentHunger <= _view.HungerRegenStage4Range.y)
                hungerMod = Stats.HealthRegenHungerStage4.Default;

            if (currentThirst <= _view.ThirstRegenStage1Range.y && currentThirst >= _view.ThirstRegenStage1Range.x)
                thirstMod = Stats.HealthRegenThirstStage1.Default;
            else if (currentThirst <= _view.ThirstRegenStage2Range.y && currentThirst >= _view.ThirstRegenStage2Range.x)
                thirstMod = Stats.HealthRegenThirstStage2.Default;
            else if (currentThirst <= _view.ThirstRegenStage3Range.y && currentThirst >= _view.ThirstRegenStage3Range.x)
                thirstMod = Stats.HealthRegenThirstStage3.Default;
            else if (currentThirst <= _view.ThirstRegenStage4Range.y)
                thirstMod = Stats.HealthRegenThirstStage4.Default;

            float currentRegen = Stats.HealthRegen.Value + hungerMod + thirstMod;
            SetAllHealths(currentRegen * Time.deltaTime);
        }

        public void SetAllHealths(float value = 0f, bool isRandomizing = false)
        {
            bool decreasing = value < 0;

            if (Stats.HeadHealth.Value < Stats.HeadHealth.Default || decreasing || isRandomizing)
                _statSetter.SetStat(Stats.HeadHealth, value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
            if (Stats.BodyHealth.Value < Stats.BodyHealth.Default || decreasing || isRandomizing)
                _statSetter.SetStat(Stats.BodyHealth, value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
            if (Stats.LeftArmHealth.Value < Stats.LeftArmHealth.Default || decreasing || isRandomizing)
                _statSetter.SetStat(Stats.LeftArmHealth, value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
            if (Stats.RightArmHealth.Value < Stats.RightArmHealth.Default || decreasing || isRandomizing)
                _statSetter.SetStat(Stats.RightArmHealth, value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
            if (Stats.LeftLegHealth.Value < Stats.LeftLegHealth.Default || decreasing || isRandomizing)
                _statSetter.SetStat(Stats.LeftLegHealth, value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
            if (Stats.RightLegHealth.Value < Stats.RightLegHealth.Default || decreasing || isRandomizing)
                _statSetter.SetStat(Stats.RightLegHealth, value - (isRandomizing ? Random.Range(1f, 5f) : 0f));
        }

        private void ProcessStamina()
        {
            if (Math.Abs(Stats.Stamina.Value - Stats.Stamina.Default) < 0.01f ||
                _playerState.MoveState == MoveState.Jump || _playerState.MoveState == MoveState.Sprint)
                return;

            float currentFatigue = Stats.Fatigue.Value / Stats.Fatigue.Default;
            float currentHunger = Stats.Hunger.Value / Stats.Hunger.Default;
            float currentThirst = Stats.Thirst.Value / Stats.Thirst.Default;

            float currentRegen =
                Stats.StaminaRegen.Value * (1 - Stats.Stamina.Value / Stats.Stamina.Default) * currentFatigue * currentHunger * currentThirst;

            _statSetter.SetStat(Stats.Stamina, currentRegen * Time.deltaTime);
        }

        private void ProcessHunger()
        {
            if (Stats.Hunger.Value <= 0)
                return;
            
            float currentHungerDecrease = Stats.HungerDecrease.Value + _relativeSpeed;
            _statSetter.SetStat(Stats.Hunger, -currentHungerDecrease * Time.deltaTime, true);
        }

        private void ProcessFatigue()
        {
            if (Stats.Fatigue.Value <= 0)
                return;

            float currentFatigueDecrease = Stats.FatigueDecrease.Value + _relativeSpeed;
            _statSetter.SetStat(Stats.Fatigue, -currentFatigueDecrease * Time.deltaTime, true);
        }

        private void ProcessThirst()
        {
            if (Stats.Thirst.Value <= 0)
                return;

            float currentThirstDecrease = Stats.ThirstDecrease.Value + _relativeSpeed;
            _statSetter.SetStat(Stats.Thirst, -currentThirstDecrease * Time.deltaTime, true);
        }

        public void Dispose()
            => _data.Save();
#if UNITY_EDITOR
        [MenuItem("Debug/Clear Save")]
        public static void ClearSave()
        {
            try
            {
                File.Delete(MainDataBase.Path);
            }
            catch (Exception e)
            {
                Debug.Log("Reload assemblies");
            }
        }
#endif
#if UNITY_EDITOR
        [MenuItem("Debug/Open save folder")]
#endif
        public static void OpenSaveFolder()
        {
            System.Diagnostics.Process.Start("explorer.exe", Application.persistentDataPath.Replace("/", "\\"));
        }
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
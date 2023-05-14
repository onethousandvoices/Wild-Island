using Effects;
using UnityEngine;
using UnityEngine.InputSystem;
using WildIsland.Controllers;
using WildIsland.Utility;
using Zenject;

namespace WildIsland.Processors
{
    public class PlayerCheatProcessor : IGetCheats
    {
        [Inject] private IGetPlayerStats _player;
        [Inject] private IDataProcessor _dataProcessor;
        [Inject] private IEffectProcessor _effectProcessor;

        private TestTemporaryEffect _testTemporary;
        private TestPeriodicEffect _testPeriodic;

        private bool _isTimeSpeedUp;
        private bool _isFrameRate60;

        public void CHEAT_TimeSpeedUp(InputAction.CallbackContext obj)
        {
            _isTimeSpeedUp = !_isTimeSpeedUp;
            Time.timeScale = _isTimeSpeedUp ? 100f : 1f;
        }

        public void CHEAT_Damage(InputAction.CallbackContext obj)
            => _dataProcessor.SetAllHealths(isRandomizing: true);

        public void CHEAT_FrameRateChange(InputAction.CallbackContext obj)
        {
            _isFrameRate60 = !_isFrameRate60;
            Debug.Log($"Current fps is {Application.targetFrameRate} trying to set {(_isFrameRate60 ? 60 : -1)}");
            Application.targetFrameRate = _isFrameRate60 ? 60 : -1;
            Debug.Log($"Fps set to {Application.targetFrameRate}");
        }

        public void CHEAT_TemporaryEffectApply(InputAction.CallbackContext obj)
        {
            _testTemporary = new TestTemporaryEffect(5f);
            _testTemporary.AffectedStats.Add(new AffectedStat(_player.Stats.HungerDecrease, 3));
            _effectProcessor.AddEffect(_testTemporary);
        }

        public void CHEAT_PeriodicEffectApply(InputAction.CallbackContext obj)
        {
            _testPeriodic = new TestPeriodicEffect(1f, 3f);
            _testPeriodic.AffectedStats.Add(new AffectedStat(_player.Stats.HeadHealth, -10));
            _effectProcessor.AddEffect(_testPeriodic);
        }
    }
    
    public interface IGetCheats
    {
        public void CHEAT_TimeSpeedUp(InputAction.CallbackContext obj);
        public void CHEAT_Damage(InputAction.CallbackContext obj);
        public void CHEAT_FrameRateChange(InputAction.CallbackContext obj);
        public void CHEAT_TemporaryEffectApply(InputAction.CallbackContext obj);
        public void CHEAT_PeriodicEffectApply(InputAction.CallbackContext obj);
    }
}
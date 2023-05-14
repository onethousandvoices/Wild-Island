using Effects;
using UnityEngine;
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

        public void TimeSpeedUp(int time)
            => Time.timeScale = time;

        public void DamagePlayer()
            => _dataProcessor.SetAllHealths(isRandomizing: true);

        public void FrameRateChange(int fps)
        {
            Application.targetFrameRate = fps;
            Debug.Log($"Fps set to {Application.targetFrameRate}");
        }

        public void TemporaryEffectApply()
        {
            _testTemporary = new TestTemporaryEffect(5f);
            _testTemporary.AffectedStats.Add(new AffectedStat(_player.Stats.HungerDecrease, 3));
            _effectProcessor.AddEffect(_testTemporary);
        }

        public void PeriodicEffectApply()
        {
            _testPeriodic = new TestPeriodicEffect(1f, 3f);
            _testPeriodic.AffectedStats.Add(new AffectedStat(_player.Stats.HeadHealth, -10));
            _effectProcessor.AddEffect(_testPeriodic);
        }
    }
    
    public interface IGetCheats
    {
        public void TimeSpeedUp(int time);
        public void DamagePlayer();
        public void FrameRateChange(int fps);
        public void TemporaryEffectApply();
        public void PeriodicEffectApply();
    }
}
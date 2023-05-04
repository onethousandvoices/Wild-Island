using System;
using WildIsland.Data;
using WildIsland.Utility;

namespace Effects
{
    public abstract class BaseEffect
    {
        public readonly AffectedStats AffectedStats;
        
        private readonly Action<PlayerData> _onApply;
        
        private bool _isInstantApplied;

        public float CurrentCooldown { get; protected set; }
        public float CurrentDuration { get; protected set; }
        
        public bool IsExecuted { get; protected set; }

        protected BaseEffect(Action<PlayerData> apply)
        {
            _onApply = apply;
            AffectedStats = new AffectedStats();
        }

        protected bool InstantApply()
        {
            if (_isInstantApplied)
                return false;
            _isInstantApplied = true;
            return _isInstantApplied;
        }
        
        public abstract bool Process();
        
        public PlayerStat[] Apply(PlayerData data)
        {
            _onApply?.Invoke(data);
            return AffectedStats.ApplyReturnStats;
        }

        public void Remove(PlayerData data)
        {
            IsExecuted = false;
            _isInstantApplied = false;
            AffectedStats.RevertClear();
        }
    }
}
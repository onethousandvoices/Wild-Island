using System;
using WildIsland.Data;

namespace Effects
{
    public abstract class BaseEffect
    {
        private readonly Func<PlayerData, PlayerStat[]> _onApply;
        private readonly Func<PlayerData, PlayerStat[]> _onRemove;

        private bool _isInstantApplied;

        public float CurrentCooldown { get; protected set; }
        public float CurrentDuration { get; protected set; }
        public bool IsExecuted { get; protected set; }

        protected BaseEffect(Func<PlayerData, PlayerStat[]> apply, Func<PlayerData, PlayerStat[]> remove)
        {
            _onApply = apply;
            _onRemove = remove;
        }

        protected bool InstantApply()
        {
            if (_isInstantApplied)
                return false;
            _isInstantApplied = true;
            return _isInstantApplied;
        }
        
        public abstract bool Process();
        
        public virtual PlayerStat[] Apply(PlayerData data)
            => _onApply(data);

        public PlayerStat[] Remove(PlayerData data)
        {
            IsExecuted = false;
            return _onRemove(data);
        }
    }
}
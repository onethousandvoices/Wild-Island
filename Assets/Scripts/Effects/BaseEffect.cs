using System;
using WildIsland.Data;

namespace Effects
{
    public abstract class BaseEffect
    {
        private readonly Func<PlayerData, PlayerStat[]> _onApply;
        private readonly Func<PlayerData, PlayerStat[]> _onRemove;

        public virtual PlayerStat[] Apply(PlayerData data)
            => _onApply(data);

        public virtual PlayerStat[] Remove(PlayerData data)
            => _onRemove(data);

        protected BaseEffect(Func<PlayerData, PlayerStat[]> apply, Func<PlayerData, PlayerStat[]> remove)
        {
            _onApply = apply;
            _onRemove = remove;
        }
    }
}
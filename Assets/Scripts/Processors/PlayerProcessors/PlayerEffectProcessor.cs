using Effects;
using System;
using System.Collections.Generic;
using WildIsland.Controllers;
using WildIsland.Data;
using WildIsland.Views;
using Zenject;

namespace WildIsland.Processors
{
    public class PlayerEffectProcessor : PlayerProcessor, IPlayerProcessor, IEffectProcessor
    {
        [Inject] private PlayerView _view;
        [Inject] private IPlayerStatSetter _statSetter;
        [Inject] private IGetPlayerStats _player;

        private List<BaseEffect> _effects;
        private BaseEffect[] _nativeEffects;

        public override void Initialize()
        {
            _effects = new List<BaseEffect>();
            _nativeEffects = Array.Empty<BaseEffect>();

            _view.SetEffectCallbacks(EffectApply, EffectRemove);
        }

        public void AddEffect(BaseEffect effect)
            => _effects.Add(effect);

        private void EffectApply(BaseEffect effect)
            => _effects.Add(effect);

        private void EffectRemove(Type effectType)
        {
            BaseEffect target = _effects.Find(x => x.GetType() == effectType);
            target.Remove(_player.Stats);
            _effects.Remove(target);
        }

        public void Tick()
        {
            if (!Enabled)
                return;
            
            if (_effects.Count < 1)
                return;
            _nativeEffects = _effects.ToArray();
            foreach (BaseEffect effect in _nativeEffects)
            {
                if (effect.Process())
                    SetStats(effect.Apply(_player.Stats));
                else if (effect.IsExecuted)
                    EffectRemove(effect.GetType());
            }
        }

        private void SetStats(PlayerStat[] affectedStats)
        {
            if (affectedStats == null)
                return;
            foreach (PlayerStat stat in affectedStats)
                _statSetter.SetStat(stat, forceDebugShow: true);
        }
    }

    public interface IEffectProcessor
    {
        public void AddEffect(BaseEffect effect);
    }
}
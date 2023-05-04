using System;
using UnityEngine;
using WildIsland.Data;

namespace Effects
{
    public abstract class TemporaryEffect : BaseEffect
    {
        protected TemporaryEffect(float duration, Action<PlayerData> apply) : base(apply)
            => CurrentDuration = duration;
        
        public override bool Process()
        {
            if (InstantApply())
                return true;

            CurrentDuration -= Time.deltaTime;
            if (CurrentDuration > 0)
                return false;
            IsExecuted = true;
            return false;
        }
    }
}
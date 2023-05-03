﻿using System;
using UnityEngine;
using WildIsland.Data;

namespace Effects
{
    public abstract class TemporaryEffect : BaseEffect
    {
        protected TemporaryEffect(float duration, Func<PlayerData, PlayerStat[]> apply, Func<PlayerData, PlayerStat[]> remove) : base(apply, remove)
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
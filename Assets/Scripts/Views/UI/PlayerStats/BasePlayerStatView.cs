using System;
using UnityEngine;
using UnityEngine.UI;
using WildIsland.Data;

namespace WildIsland.Views.UI
{
    public abstract class BasePlayerStatView : MonoBehaviour
    {
        private Slider _slider;
        private PlayerStat _current;
        private PlayerStat _max;

        public abstract Type TargetStat { get; }

        public void SetRefs(PlayerStat current, PlayerStat max)
        {
            _current = current;
            _max = max;
        }

        public void Update()
            => _slider.value = _current.Value / _max.Value;

        private void OnValidate()
            => _slider = GetComponentInChildren<Slider>();
    }
}
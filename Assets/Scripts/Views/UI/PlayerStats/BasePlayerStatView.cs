using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WildIsland.Data;

namespace WildIsland.Views.UI
{
    public abstract class BasePlayerStatView : MonoBehaviour
    {
        private Slider _slider;
        private PlayerStat _stat;
        private TextMeshProUGUI _debugText;

        private const float _baseDebugDelay = 0.07f;
        private float _currentDebugDelay;
        
        public void SetRefs(PlayerStat stat)
        {
            _stat = stat;
            _currentDebugDelay = _baseDebugDelay;
        }

        public void UpdateDebugValue(float debug_currentValue, bool forceShow)
        {
            if (forceShow)
            {
                _currentDebugDelay = _baseDebugDelay * 6f;
                _debugText.color = Color.red;
                _debugText.text = debug_currentValue.ToString("N" + 3);
                return;
            }
            
            _currentDebugDelay -= Time.deltaTime;
            if (_currentDebugDelay > 0)
                return;
            _currentDebugDelay = _baseDebugDelay;
            _debugText.color = Color.white;
            _debugText.text = debug_currentValue.ToString("N" + 3);
        }

        public void Update()
            => _slider.value = _stat.Value / _stat.Default;

        private void OnValidate()
        {
            _slider = GetComponentInChildren<Slider>();

            foreach (Transform child in transform)
            {
                if (child.name != "Debug")
                    continue;

                _debugText = child.GetComponentInChildren<TextMeshProUGUI>();
            }
        }
    }
}
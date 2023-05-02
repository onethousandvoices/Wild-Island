using UnityEngine;
using UnityEngine.UI;

namespace Views.UI
{
    public class DayTimerView : MonoBehaviour
    {
        [SerializeField] private Slider _daySlider;
        [SerializeField] private Slider _nightSlider;

        public void SetDayTime(float time)
            => _daySlider.value = 1 - time;

        public void SetNightTime(float time)
            => _nightSlider.value = 1 - time;
    }
}
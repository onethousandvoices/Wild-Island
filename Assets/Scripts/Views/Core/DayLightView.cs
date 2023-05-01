using UnityEngine;

namespace WildIsland.Views
{
    public class DayLightView : MonoBehaviour
    {
        [SerializeField] private Light _sun;
        [SerializeField] private Light _moon;
        [SerializeField] private AnimationCurve _sunCurve;
        [SerializeField] private AnimationCurve _moonCurve;
        [SerializeField] private Material _daySkybox;
        [SerializeField] private Material _nightSkybox;

        public float SunIntensity => _sun.intensity;
        public float MoonIntensity => _moon.intensity;
        public Light Sun => _sun;
        public Light Moon => _moon;
        public AnimationCurve SunCurve => _sunCurve;
        public AnimationCurve MoonCurve => _moonCurve;
        public Material DaySkybox => _daySkybox;
        public Material NightSkybox => _nightSkybox;

        public void SetSunParams(float time, float intensity)
        {
            _sun.transform.rotation = Quaternion.Euler(time * 360f, 180f, 0f);
            _sun.intensity = intensity;
        }

        public void SetMoonParams(float time, float intensity)
        {
            _moon.transform.rotation = Quaternion.Euler(time * 360f + 180f, 180f, 0f);
            _moon.intensity = intensity;
        }
    }
}
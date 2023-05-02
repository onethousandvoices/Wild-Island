using NaughtyAttributes;
using UnityEngine;

namespace WildIsland.Views
{
    public class DayLightView : MonoBehaviour
    {
        [field: SerializeField, MinMaxSlider(0f, 100f), HorizontalLine(color: EColor.Blue)] public Vector2 DayTemperatureAffectStage1 { get; private set; }
        [field: SerializeField, MinMaxSlider(0f, 100f)] public Vector2 DayTemperatureAffectStage2 { get; private set; }
        [field: SerializeField, MinMaxSlider(0f, 100f)] public Vector2 DayTemperatureAffectStage3 { get; private set; }
        [field: SerializeField, MinMaxSlider(0f, 100f)] public Vector2 NightTemperatureAffectStage1 { get; private set; }
        [field: SerializeField, MinMaxSlider(0f, 100f)] public Vector2 NightTemperatureAffectStage2 { get; private set; }
        [field: SerializeField, HorizontalLine(color: EColor.Red)] public Light Sun { get; private set; }
        [field: SerializeField] public Light Moon { get; private set; }
        [field: SerializeField] public AnimationCurve SunCurve { get; private set; }
        [field: SerializeField] public AnimationCurve MoonCurve { get; private set; }
        [field: SerializeField] public Material DaySkybox { get; private set; }
        [field: SerializeField] public Material NightSkybox { get; private set; }

        public float SunIntensity => Sun.intensity;
        public float MoonIntensity => Moon.intensity;

        public void SetSunParams(float time, float intensity)
        {
            Sun.transform.rotation = Quaternion.Euler(time * 360f, 180f, 0f);
            Sun.intensity = intensity;
        }

        public void SetMoonParams(float time, float intensity)
        {
            Moon.transform.rotation = Quaternion.Euler(time * 360f + 180f, 180f, 0f);
            Moon.intensity = intensity;
        }
    }
}
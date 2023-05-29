using NaughtyAttributes;
using UnityEngine;
using WildIsland.Data;
using WildIsland.SOs;

namespace WildIsland.Views
{
    public class DayLightView : MonoBehaviour, IDaySetter
    {
        [field: SerializeField, MinMaxSlider(0f, 100f), HorizontalLine(color: EColor.Blue)] public Vector2 DayTemperatureAffectStage1 { get; private set; }
        [field: SerializeField, MinMaxSlider(0f, 100f)] public Vector2 DayTemperatureAffectStage2 { get; private set; }
        [field: SerializeField, MinMaxSlider(0f, 100f)] public Vector2 DayTemperatureAffectStage3 { get; private set; }
        [field: SerializeField, MinMaxSlider(0f, 100f)] public Vector2 NightTemperatureAffectStage1 { get; private set; }
        [field: SerializeField, MinMaxSlider(0f, 100f)] public Vector2 NightTemperatureAffectStage2 { get; private set; }
        [field: SerializeField, HorizontalLine(color: EColor.Red)] public Light Directional { get; private set; }
        [SerializeField] private DayPreset _dayPreset;
        [SerializeField] private DayPreset _nightPreset;
        [SerializeField] private ParticleSystem _leaves;

        // [field: SerializeField] public AnimationCurve SunCurve { get; private set; }
        // [field: SerializeField] public AnimationCurve MoonCurve { get; private set; }
        // [field: SerializeField] public Material DaySkybox { get; private set; }
        // [field: SerializeField] public Material NightSkybox { get; private set; }
        // [field: SerializeField] public Color DayAmbientLight { get; private set; }
        // [field: SerializeField] public Color NightAmbientLight { get; private set; }
        
        
        public void SetSunParams(float time, float intensity)
        {
            // Sun.transform.rotation = Quaternion.Euler(time * 360f, 180f, 0f);
            // Sun.intensity = intensity;
        }

        public void SetMoonParams(float time, float intensity)
        {
            // Moon.transform.rotation = Quaternion.Euler(time * 360f + 180f, 180f, 0f);
            // Moon.intensity = intensity;
        }
        

        public void SetPreset(DayPresetType type)
        {
            DayPreset current = type == DayPresetType.Day ? _dayPreset : _nightPreset;
            
            RenderSettings.skybox = current.SkyboxMaterial;
            RenderSettings.ambientIntensity = current.LightingIntensityMultiplier;
            Directional.intensity = current.DirectionalLightIntensity;
            Directional.color = current.DirectionalLightColor;

            if (_leaves != null)
            {
                if (current.LeavesParticlesState)
                    _leaves.Play();
                else
                    _leaves.Stop();
            }
            
            DynamicGI.UpdateEnvironment();
            Debug.Log($"Day preset was set to {type}");
        }
    }
    
    public interface IDaySetter
    {
        public void SetPreset(DayPresetType type);
    }
}
using UnityEngine;

namespace WildIsland.SOs
{
    public enum DayPresetType : byte
    {
        Day,
        Night
    }
    
    [CreateAssetMenu(fileName = "Day Preset", menuName = "SOs/Day Preset")]
    public class DayPreset : ScriptableObject
    {
        [field: SerializeField] public DayPresetType Type { get; private set; }
        [field: SerializeField] public Material SkyboxMaterial { get; private set; }
        [field: SerializeField, Range(0f, 8f)] public float LightingIntensityMultiplier { get; private set; }
        [field: SerializeField, Range(0f, 5f)] public float DirectionalLightIntensity { get; private set; }
        [field: SerializeField] public Color DirectionalLightColor { get; private set; }
        [field: SerializeField] public bool LeavesParticlesState { get; private set; }
    }
}
using Effects;
using System.Globalization;
using TMPro;
using UnityEngine;
using WildIsland.Data;
using WildIsland.Views;
using Zenject;

namespace Views.Biomes
{
    public abstract class BaseBiomeView : MonoBehaviour
    {
        [Inject] private Camera _cam;

        private BiomeEffect _effect;
        private BiomeData _data;
        private TextMeshProUGUI _text;
        private Canvas _canvas;
        private BiomeEffect _biomeEffect => _effect ??= new BiomeEffect(_data);

        public void SetData(BiomeData data)
        {
            _data = data;
            _text.text = _data.Temperature.ToString(CultureInfo.InvariantCulture);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IEffectReceiver receiver))
                return;
            receiver.OnEffectApplied?.Invoke(_biomeEffect);
            Debug.Log($"{name} affecting");
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out IEffectReceiver receiver))
                return;
            receiver.OnEffectRemoved?.Invoke(_biomeEffect);
            Debug.Log($"{name} removing effect");
        }

        private void FixedUpdate()
            => _canvas.transform.LookAt(transform.position + _cam.transform.rotation * Vector3.back, _cam.transform.rotation * Vector3.up);

        private void OnValidate()
        {
            _canvas = GetComponentInChildren<Canvas>();
            _text = GetComponentInChildren<TextMeshProUGUI>();
        }
    }
}
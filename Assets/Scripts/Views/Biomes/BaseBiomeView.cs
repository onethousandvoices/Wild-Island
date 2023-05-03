using Effects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        private BiomeData _data;
        private PermanentEffect _biomeEffect;
        private List<(PlayerStat, float)> _affectedStats;
        private TextMeshProUGUI _text;
        private Canvas _canvas;
        private PlayerStat[] ConvertAffectedToArray => _affectedStats.Select(x => x.Item1).ToArray();

        private float _previousMod;

        public void Init(BiomeData data)
        {
            _data = data;
            _text.text = _data.Temperature.ToString(CultureInfo.InvariantCulture);
            _biomeEffect = new PermanentEffect(ApplyEffect, RemoveEffect);
            _affectedStats = new List<(PlayerStat, float)>();
        }

        private PlayerStat[] ApplyEffect(PlayerData playerData)
        {
            _affectedStats.Clear();

            if (_data.Temperature < playerData.Temperature.Value)
            {
                float currentEffect =
                    Math.Abs(_data.Temperature - playerData.Temperature.Value) * playerData.HungerDecrease.Value;
                _affectedStats.Add((playerData.HungerDecrease, currentEffect));
            }
            else if (_data.Temperature > playerData.Temperature.Value)
            {
                float currentEffect =
                    Math.Abs(_data.Temperature - playerData.Temperature.Value) * playerData.ThirstDecrease.Value;
                _affectedStats.Add((playerData.ThirstDecrease, currentEffect));
            }

            foreach ((PlayerStat, float) pair in _affectedStats)
                pair.Item1.Value += pair.Item2;

            return ConvertAffectedToArray;
        }

        private PlayerStat[] RemoveEffect(PlayerData playerData)
        {
            foreach ((PlayerStat, float) pair in _affectedStats)
                playerData.GetStatByType(pair.Item1.GetType()).Value -= pair.Item2;
            return ConvertAffectedToArray;
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

        public void UpdateTemperature(float mod)
        {
            if (_previousMod < 0)
                _data.Temperature += Math.Abs(_previousMod);
            else
                _data.Temperature -= _previousMod;
            _data.Temperature += mod;
            _previousMod = mod;
            _text.text = _data.Temperature.ToString(CultureInfo.InvariantCulture);
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
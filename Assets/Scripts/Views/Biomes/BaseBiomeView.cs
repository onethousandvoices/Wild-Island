﻿using Effects;
using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using WildIsland.Data;
using WildIsland.Utility;
using WildIsland.Views;
using Zenject;

namespace Views.Biomes
{
    public abstract class BaseBiomeView : MonoBehaviour
    {
        [Inject] private Camera _cam;

        private BiomeData _data;
        private BiomeEffect _biomeEffect;
        private TextMeshProUGUI _text;
        private Canvas _canvas;

        private float _previousMod;

        public void Init(BiomeData data)
        {
            _data = data;
            _text.text = _data.Temperature.ToString(CultureInfo.InvariantCulture);
            _biomeEffect = new BiomeEffect(ApplyEffect);
        }

        private void ApplyEffect(PlayerData playerData)
        {
            if (_data.Temperature < playerData.Temperature.Value)
            {
                float currentEffect =
                    Math.Abs(_data.Temperature - playerData.Temperature.Value) * playerData.HungerDecrease.Value;
                _biomeEffect.AffectedStats.Add(new AffectedStat(playerData.HungerDecrease, currentEffect));
            }
            else if (_data.Temperature > playerData.Temperature.Value)
            {
                float currentEffect =
                    Math.Abs(_data.Temperature - playerData.Temperature.Value) * playerData.ThirstDecrease.Value;
                _biomeEffect.AffectedStats.Add(new AffectedStat(playerData.ThirstDecrease, currentEffect));
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IEffectReceiver receiver))
                return;
            receiver.OnEffectApplied?.Invoke(_biomeEffect);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out IEffectReceiver receiver))
                return;
            receiver.OnEffectRemoved?.Invoke(typeof(BiomeEffect));
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
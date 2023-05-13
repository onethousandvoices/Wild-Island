using System;
using UnityEngine;
using Views.UI;
using WildIsland.Data;
using WildIsland.Views;
using Zenject;

namespace WildIsland.Controllers
{
    public class DayController : IInitializable, ITickable, IGDConsumer
    {
        [Inject] private DayLightView _view;
        [Inject] private DayTimerView _dayTimer;
        [Inject] private IBiomeDayAffect _biomeController;

        private BasicGameData.DaySettings _daySettings;
        private Material _skyBoxMaterial;

        private float _dayTime;
        private float _nightTime;
        private float _skyboxLerp;
        private float _dayDuration;
        private float _nightDuration;
        private float _currentTemperatureMod;
        
        private static readonly int GlobalSunDirection = Shader.PropertyToID("GlobalSunDirection");

        public Type ContainerType => typeof(BasicGameDataContainer);

        public void AcquireGameData(IPartialGameDataContainer container)
            => _daySettings = ((BasicGameDataContainer)container).Default.DaySettingsData;

        public void Initialize()
        {
            _dayDuration = _daySettings.DayTimer;
            _nightDuration = _daySettings.NightTimer;

            _skyBoxMaterial = new Material(_view.DaySkybox);
        }

        public void Tick()
        {
            return;
            
            CheckTemperatureEffectRanges();

            if (_dayTime < 1)
                _dayTime += Time.deltaTime / _dayDuration;
            else if (_nightTime < 1)
                _nightTime += Time.deltaTime / _nightDuration;
            else
            {
                _dayTime = 0;
                _nightTime = 0;
            }

            _dayTimer.SetDayTime(_dayTime);
            _dayTimer.SetNightTime(_nightTime);

            float dotProduct = Vector3.Dot(_view.Sun.transform.forward, Vector3.down);
            float sunIntensity = Mathf.Lerp(0f, 1f, dotProduct);
            float moonIntensity = Mathf.Lerp(0.5f, 0f, dotProduct);
            RenderSettings.ambientLight = Color.Lerp(_view.DayAmbientLight, _view.NightAmbientLight, dotProduct);
            
            float evaluation = (_dayTime + _nightTime) / 2;

            _view.SetSunParams(evaluation, sunIntensity);
            _view.SetMoonParams(evaluation, moonIntensity);

            _skyboxLerp = evaluation switch
                          {
                              >= 0.5f => 2 - evaluation * 2,
                              <= 0.5f => evaluation * 2,
                              _       => _skyboxLerp
                          };

            _skyBoxMaterial.Lerp(_view.DaySkybox, _view.NightSkybox, _skyboxLerp);
            RenderSettings.skybox = _skyBoxMaterial;
            RenderSettings.sun = evaluation > 0.5f ? _view.Moon : _view.Sun;

            Shader.SetGlobalVector(GlobalSunDirection, -(evaluation > 0.5f ? _view.Moon : _view.Sun).transform.forward);

            DynamicGI.UpdateEnvironment();
        }

        private void CheckTemperatureEffectRanges()
        {
            float previousValue = _currentTemperatureMod;
            float dayRelativeValue = (1 - _dayTime) * 100;
            float nightRelativeValue = (1 - _nightTime) * 100;

            if (dayRelativeValue > 0)
            {
                if (dayRelativeValue < _view.DayTemperatureAffectStage1.y && dayRelativeValue > _view.DayTemperatureAffectStage1.x)
                    _currentTemperatureMod = _daySettings.DayTemperatureAffectStage1;
                else if (dayRelativeValue < _view.DayTemperatureAffectStage2.y && dayRelativeValue > _view.DayTemperatureAffectStage2.x)
                    _currentTemperatureMod = _daySettings.DayTemperatureAffectStage2;
                else if (dayRelativeValue < _view.DayTemperatureAffectStage3.y && dayRelativeValue > _view.DayTemperatureAffectStage3.x)
                    _currentTemperatureMod = _daySettings.DayTemperatureAffectStage3;
            }
            else if (nightRelativeValue > 0)
            {
                if (nightRelativeValue < _view.NightTemperatureAffectStage1.y && nightRelativeValue > _view.NightTemperatureAffectStage1.x)
                    _currentTemperatureMod = _daySettings.NightTemperatureAffectStage1;
                else if (nightRelativeValue < _view.NightTemperatureAffectStage2.y && nightRelativeValue > _view.NightTemperatureAffectStage2.x)
                    _currentTemperatureMod = _daySettings.NightTemperatureAffectStage2;
            }

            if (Math.Abs(_currentTemperatureMod - previousValue) > 0.01f)
                DayBiomeEffectChanged();
        }

        private void DayBiomeEffectChanged()
            => _biomeController.UpdateBiomesTemperature(_currentTemperatureMod);
    }
}
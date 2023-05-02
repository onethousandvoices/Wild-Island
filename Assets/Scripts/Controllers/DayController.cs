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

        private BasicGameData.DaySettings _daySettings;

        private float _dayTime;
        private float _nightTime;
        private float _sunIntensity;
        private float _moonIntensity;
        private float _skyboxLerp;

        private float _dayDuration;
        private float _nightDuration;

        public Type ContainerType => typeof(BasicGameDataContainer);
        public void AcquireGameData(IPartialGameDataContainer container)
            => _daySettings = ((BasicGameDataContainer)container).Default.DaySettingsData;

        public void Initialize()
        {
            _dayDuration = _daySettings.DayTimer;
            _nightDuration = _daySettings.NightTimer;
            _sunIntensity = _view.SunIntensity;
            _moonIntensity = _view.MoonIntensity;
        }

        public void Tick()
        {
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
            
            float evaluation = (_dayTime + _nightTime) / 2;

            _view.SetSunParams(evaluation, _sunIntensity * _view.SunCurve.Evaluate(_dayTime));
            _view.SetMoonParams(evaluation, _moonIntensity * _view.MoonCurve.Evaluate(_nightTime));

            _skyboxLerp = evaluation switch
                          {
                              >= 0.5f => 2 - evaluation * 2,
                              <= 0.5f => evaluation * 2,
                              _       => _skyboxLerp
                          };

            RenderSettings.skybox.Lerp(_view.DaySkybox, _view.NightSkybox, _skyboxLerp);
            RenderSettings.sun = evaluation > 0.5f ? _view.Moon : _view.Sun;
            DynamicGI.UpdateEnvironment();
        }
    }
}
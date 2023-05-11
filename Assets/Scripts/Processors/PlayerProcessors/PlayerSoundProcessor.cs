using UnityEngine;
using WildIsland.Views;
using Zenject;

namespace WildIsland.Processors
{
    public class PlayerSoundProcessor : PlayerProcessor
    {
        [Inject] private PlayerView _view;
        
        private const float _footstepAudioVolume = 0.5f;

        public override void Initialize()
        {
            _view.SetOnLandCallback(Land);
            _view.SetOnFootStepCallback(Footstep);
        }

        private void Footstep(AnimationEvent animationEvent)
        {
            if (!(animationEvent.animatorClipInfo.weight > 0.5f))
                return;
            if (_view.FootstepAudioClips.Length <= 0)
                return;
            int index = Random.Range(0, _view.FootstepAudioClips.Length);
            AudioSource.PlayClipAtPoint(_view.FootstepAudioClips[index], _view.transform.TransformPoint(_view.transform.position), _footstepAudioVolume);
        }

        private void Land(AnimationEvent animationEvent)
        {
            if (!(animationEvent.animatorClipInfo.weight > 0.5f))
                return;
            AudioSource.PlayClipAtPoint(_view.LandingAudioClip, _view.transform.TransformPoint(_view.transform.position), _footstepAudioVolume);
        }
    }
}
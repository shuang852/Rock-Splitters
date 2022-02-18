using Audio;
using Managers;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Pause
{
    public class AudioSlider : DialogueComponent<Dialogue>
    {
        [SerializeField] private Bus bus;
        private Slider slider;
        private AudioManager manager;

        protected override void OnComponentAwake()
        {
            manager = M.GetOrThrow<AudioManager>();
            TryGetComponent(out slider);
            slider.value = manager.GetVolume(bus);
        }

        protected override void Subscribe() { }

        protected override void Unsubscribe() { }

        public void ChangeVolume(float value) => manager.ChangeVolume(bus, value);
    }
}

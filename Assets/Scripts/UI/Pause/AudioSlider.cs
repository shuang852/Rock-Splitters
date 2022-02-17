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

        // Start is called before the first frame update
        private void Start()
        {
            manager = M.GetOrThrow<AudioManager>();
            slider.value = manager.GetVolume(bus);
        }

        protected override void Subscribe() { }

        protected override void Unsubscribe() { }

        public void ChangeVolume(float value) => manager.ChangeVolume(bus, value);
    }
}

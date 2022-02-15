using Managers;
using UI.Core;
using UI.Generic;
using UnityEngine;

namespace UI.Pause
{
    public class AudioSlider : MonoBehaviour
    {
        [SerializeField] private Bus bus;
        public bool PersistBetweenScenes => true;
        private AudioManager manager;

        // Start is called before the first frame update
        void Start()
        {
            manager = M.GetOrThrow<AudioManager>();
        }

        public void ChangeVolume(float value)
        {
            manager.ChangeVolume(bus, value);
        }
    }
}

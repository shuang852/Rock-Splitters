using System;
using UnityEngine;

namespace Audio
{
    [Serializable]
    public class AudioBus
    {
        public Bus enumValue;

        public float Volume
        {
            get => volume;
            set
            {
                volume = value;
                bus.setVolume(volume);
            }
        }

        [SerializeField] private string path;
        
        private FMOD.Studio.Bus bus;
        private float volume;

        public void Initialise()
        {
            bus = FMODUnity.RuntimeManager.GetBus(path);

            Volume = 1;
        }
    }
}
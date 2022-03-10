using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Audio
{
    public class AudioManager : Manager
    {
        [SerializeField] private List<AudioBus> audioBuses;

        public override bool PersistBetweenScenes => true;

        protected override void Start()
        {
            base.Start();
            
            audioBuses.ForEach(a => a.Initialise());
        }

        public void ChangeVolume(Bus bus, float value)
        {
            var audioBus = GetAudioBus(bus);
            
            if (audioBus == null) return;

            audioBus.Volume = value;
        }

        public float GetVolume(Bus bus)
        {
            var audioBus = GetAudioBus(bus);
            
            if (audioBus == null) return 0;

            return audioBus.Volume;
        }

        private AudioBus GetAudioBus(Bus bus)
        {
            return audioBuses.Find(a => a.enumValue == bus);
        }
    }
}
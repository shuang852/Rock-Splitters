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
            
            LoadSettings();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            SaveSettings();
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

        private void SaveSettings()
        {
            foreach (var audioBus in audioBuses)
            {
                PlayerPrefs.SetFloat(PrefName(audioBus), audioBus.Volume);
            }
        }

        private void LoadSettings()
        {
            foreach (var audioBus in audioBuses)
            {
                if (!PlayerPrefs.HasKey(PrefName(audioBus))) continue;
                
                audioBus.Volume = PlayerPrefs.GetFloat(PrefName(audioBus));
            }
            
            PlayerPrefs.Save();
        }

        private static string PrefName(AudioBus audioBus)
        {
            return "Audio" + audioBus.enumValue;
        }
    }
}
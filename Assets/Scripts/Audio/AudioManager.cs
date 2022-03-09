using System;
using System.Collections.Generic;
using Managers;

namespace Audio
{
    public class AudioManager : Manager
    {
        private FMOD.Studio.Bus master;
        private FMOD.Studio.Bus music;
        private FMOD.Studio.Bus sfx;
        
        private float masterVol = 1;
        private float musicVol =1;
        private float sfxVol =1;

        public override bool PersistBetweenScenes => true; 

        protected override void Start()
        {
            base.Start();
            master = FMODUnity.RuntimeManager.GetBus("bus:/");
            music = FMODUnity.RuntimeManager.GetBus("bus:/Music");
            sfx = FMODUnity.RuntimeManager.GetBus("bus:/SFX");

            master.setVolume(masterVol);
            music.setVolume(musicVol);
            sfx.setVolume(sfxVol);
        }

        public void ChangeVolume(Bus bus, float value)
        {
            switch (bus)
            {
                case Bus.Master:
                    master.setVolume(value);
                    masterVol = value;
                    break;
                case Bus.Music:
                    music.setVolume(value);
                    musicVol = value;
                    break;
                case Bus.SFX:
                    sfx.setVolume(value);
                    sfxVol = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bus), bus, null);
            }
        }

        public float GetVolume(Bus bus)
        {
            return bus switch
            {
                Bus.Master => masterVol,
                Bus.Music => musicVol,
                Bus.SFX => sfxVol,
                _ => throw new ArgumentOutOfRangeException(nameof(bus), bus, null)
            };
        }
    }
}
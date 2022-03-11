using FMOD.Studio;
using UnityEngine;

namespace Audio
{
    public class PlayOneShot : MonoBehaviour
    {
        [SerializeField] private FMODUnity.EventReference fmodEvent;

        public PLAYBACK_STATE PlaybackState
        {
            get
            {
                instance.getPlaybackState(out var pS);
                return pS;
            }
        }
        private EventInstance instance;
        
        public void PlayOnce()
        {
            instance.start();
        }

        public void StopImmediate() => instance.stop(STOP_MODE.IMMEDIATE);

        public void StopFadeOut() => instance.stop(STOP_MODE.ALLOWFADEOUT);

        private void OnEnable()
        {
            instance = FMODUnity.RuntimeManager.CreateInstance(fmodEvent);
            StopImmediate();
        }

        private void OnDisable()
        {
            instance.release();
        }
    }
}
using FMOD.Studio;
using UnityEngine;

namespace Audio
{
    public class PlayOneShot : MonoBehaviour
    {
        [SerializeField] private FMODUnity.EventReference fmodEvent;

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
        }

        private void OnDisable()
        {
            instance.release();
        }
    }
}
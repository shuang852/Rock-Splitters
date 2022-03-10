using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Audio
{
    public class ChunkAudio : MonoBehaviour
    {
        [SerializeField] private EventReference fmodEvent;

        private static bool playing;
        public float delay;

        public async void PlayAudio()
        {
            if (playing) return;
            
            playing = true;
            var instance = RuntimeManager.CreateInstance(fmodEvent);
            instance.start();
            instance.release();
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            playing = false;
        }

    }
}
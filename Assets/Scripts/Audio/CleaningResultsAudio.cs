using System;
using Cleaning;
using Cysharp.Threading.Tasks;
using Managers;
using Stored;
using UnityEngine;

namespace Audio
{
    public class CleaningResultsAudio : MonoBehaviour
    {
        [SerializeField] private PlayOneShot success;
        [SerializeField] private PlayOneShot failed;
        [SerializeField] private float delay = 0.1f;
        private CleaningManager cleaningManager;

        private void Start()
        {
            cleaningManager = M.GetOrThrow<CleaningManager>();

            cleaningManager.artefactRockSucceeded.AddListener(PlaySuccess);
            cleaningManager.artefactRockFailed.AddListener(PlayFailed);
        }

        private async void PlaySuccess()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            success.PlayOnce();
        }
        
        private async void PlayFailed()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            failed.PlayOnce();
        }
    }
}
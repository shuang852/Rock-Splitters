using System;
using Cysharp.Threading.Tasks;
using UI.Core;
using DG.Tweening;
using UnityEngine;

namespace UI.Cleaning_Results
{
    public class CleaningFossilResultsDialogue : Dialogue
    {
        [Header("Fade Timings")] 
        [SerializeField]private float fadeDuration;
        [SerializeField]private float fadeWait ;

        public float TotalDuration => fadeDuration * 2 + fadeWait;

        [ContextMenu("Close")]
        protected override void OnClose()
        {
        }

        protected override async void OnPromote()
        { 
            await canvasGroup.DOFade(1, fadeDuration).AsyncWaitForCompletion();
            await UniTask.Delay(TimeSpan.FromSeconds(fadeWait));
            canvasGroup.DOFade(0, fadeDuration);
            //OnDemote();
        }

        protected override void OnDemote()
        {
            
        }
    }
}
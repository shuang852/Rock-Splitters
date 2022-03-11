using System;
using Cleaning;
using Cysharp.Threading.Tasks;
using Managers;
using TMPro;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CleaningCountdown
{
    public class CountdownText : DialogueComponent<CleaningCountdownDialogue>
    {
        [SerializeField] private int countdownSteps;
        [SerializeField] private float stepDuration;
        [SerializeField] private string goString;
        [SerializeField] private TMP_Text text;

        private CleaningManager cleaningManager;

        protected override void OnComponentAwake()
        {
            base.OnComponentAwake();
            
            // TODO: This is risky.
            cleaningManager = M.GetOrThrow<CleaningManager>();
            
            Countdown();
        }

        private async void Countdown()
        {
            cleaningManager.PauseCleaning();
            
            for (int i = countdownSteps; i > 0; i--)
            {
                text.text = i.ToString();
                
                await UniTask.Delay(TimeSpan.FromSeconds(stepDuration), DelayType.DeltaTime);
            }

            text.text = goString;
            
            await UniTask.Delay(TimeSpan.FromSeconds(stepDuration), DelayType.Realtime);
            
            cleaningManager.ResumeCleaning();
            
            Manager.Pop();
        }

        protected override void Subscribe() { }

        protected override void Unsubscribe() { }
    }
}
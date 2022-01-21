using System;
using System.Globalization;
using Cleaning;
using Managers;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Cleaning
{
    public class CleaningTimerUI : DialogueComponent<CleaningDialogue>
    {
        private CleaningTimerManager timer;
        private Text text;

        protected override void OnComponentAwake()
        {
            TryGetComponent(out text);
        }

        protected override void OnComponentStart()
        {
            base.OnComponentStart();

            timer = M.GetOrThrow<CleaningTimerManager>();
            
            timer.TimeChanged.AddListener(OnTimeChanged);
        }

        protected override void Subscribe() { }

        protected override void Unsubscribe() { }
        
        private void OnTimeChanged()
        {
            text.text = timer.CurrentTime.ToString(CultureInfo.InvariantCulture);
        }

        private void OnDestroy()
        {
            timer.TimeChanged.RemoveListener(OnTimeChanged);
        }
    }
}
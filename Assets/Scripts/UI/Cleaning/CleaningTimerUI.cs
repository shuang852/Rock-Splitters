using System.Globalization;
using Cleaning;
using Managers;
using TMPro;
using UI.Core;
using UnityEngine;

namespace UI.Cleaning
{
    public class CleaningTimerUI : DialogueComponent<CleaningDialogue>
    {
        private CleaningTimerManager timer;
        private TextMeshProUGUI text;

        protected override void OnComponentAwake()
        {
            TryGetComponent(out text);
        }

        protected override void OnComponentStart()
        {
            base.OnComponentStart();

            timer = M.GetOrThrow<CleaningTimerManager>();
            
            timer.timeChanged.AddListener(OnTimeChanged);
        }

        protected override void Subscribe() { }

        protected override void Unsubscribe() { }
        
        private void OnTimeChanged()
        {
            text.text = timer.CurrentTime.ToString("F2", CultureInfo.InvariantCulture);
        }

        private void OnDestroy()
        {
            timer.timeChanged.RemoveListener(OnTimeChanged);
        }
    }
}
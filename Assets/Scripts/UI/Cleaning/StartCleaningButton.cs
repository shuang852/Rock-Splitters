using Cleaning;
using Managers;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Cleaning
{
    public class StartCleaningButton : DialogueComponent<CleaningDialogue>
    {
        private Button button;

        private CleaningManager cleaningManager;

        protected override void OnComponentStart()
        {
            base.OnComponentStart();

            cleaningManager = M.GetOrThrow<CleaningManager>();
        }

        protected override void OnComponentAwake()
        {
            TryGetComponent(out button);
            button.onClick.AddListener(OnSubmit);
        }
        
        protected override void Subscribe() { }

        protected override void Unsubscribe() { }
        
        private void OnSubmit()
        {
            cleaningManager.StartCleaning();
        }
    }
}
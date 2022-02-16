using Cleaning;
using Managers;
using UI.Core;
using UnityEngine.UI;

namespace UI.Cleaning
{
    public class StartCleaningButton : DialogueComponent<CleaningDialogue>
    {
        private Button button;

        private CleaningManager cleaningManager;
        
        protected override void OnComponentAwake()
        {
            TryGetComponent(out button);
        }

        protected override void OnComponentStart()
        {
            base.OnComponentStart();

            cleaningManager = M.GetOrThrow<CleaningManager>();
        }

        protected override void Subscribe()
        {
            button.onClick.AddListener(OnSubmit);
        }

        protected override void Unsubscribe()
        {
            button.onClick.RemoveListener(OnSubmit);
        }
        
        private void OnSubmit()
        {
            cleaningManager.StartCleaning();
        }
    }
}
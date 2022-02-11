using Managers;
using ToolSystem;
using UI.Core;
using UnityEngine.UI;

namespace UI.Cleaning
{
    public class XRayButton : DialogueComponent<CleaningDialogue>
    {
        private Button button;

        private XRayManager cleaningManager;
        
        protected override void OnComponentAwake()
        {
            TryGetComponent(out button);
        }

        protected override void OnComponentStart()
        {
            base.OnComponentStart();

            cleaningManager = M.GetOrThrow<XRayManager>();
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
            cleaningManager.ShowXRay();
        }
    }
}
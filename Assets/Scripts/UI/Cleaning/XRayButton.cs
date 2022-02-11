using Cleaning;
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
            button.onClick.AddListener(OnSubmit);
        }

        protected override void OnComponentStart()
        {
            base.OnComponentStart();

            cleaningManager = M.GetOrThrow<XRayManager>();
        }
        
        protected override void Subscribe() { }

        protected override void Unsubscribe() { }
        
        private void OnSubmit()
        {
            cleaningManager.ShowXRay();
        }
    }
}
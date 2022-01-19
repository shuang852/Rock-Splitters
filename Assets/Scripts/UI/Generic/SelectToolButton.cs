using Managers;
using ToolSystem;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Generic
{
    [RequireComponent(typeof(Button))]
    public class SelectToolButton : DialogueComponent<Dialogue>
    {
        [SerializeField] private Tool tool;

        private ToolManager toolManager;

        private Button button;

        protected override void OnComponentAwake()
        {
            TryGetComponent(out button);
            button.onClick.AddListener(OnSubmit);
        }

        protected override void OnComponentStart()
        {
            base.OnComponentStart();
            
            toolManager = M.GetOrThrow<ToolManager>();
        }

        protected override void Subscribe() { }

        protected override void Unsubscribe() { }

        private void OnSubmit()
        {
            if (toolManager.CurrentTool != tool)
            {
                toolManager.SelectTool(tool);
            }
        }
    }
}
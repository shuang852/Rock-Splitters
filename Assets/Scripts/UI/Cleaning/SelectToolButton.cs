using Managers;
using ToolSystem;
using UI.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Cleaning
{
    [RequireComponent(typeof(Button))]
    public class SelectToolButton : DialogueButton<CleaningDialogue>
    {
        [SerializeField] private Tool tool;
        [SerializeField] private Color activatedColor;
        [SerializeField] private Sprite unpushedButtonSprite;
        [SerializeField] private Sprite pushedButtonSprite;
        
        private ToolManager toolManager;
        
        private Image image;

        protected override void OnComponentAwake()
        {
            base.OnComponentAwake();
            
            if (!tool || !tool.unlocked) button.interactable = false;
        }

        protected override void OnComponentStart()
        {
            base.OnComponentStart();
            
            toolManager = M.GetOrThrow<ToolManager>();
            image = GetComponent<Image>();

            // Selects tool if its the starting tool. If multiple, whatever gets first then others will be set false.
            if (!tool || !tool.startingTool) return;
            
            if (toolManager.CurrentTool == null)
                SelectTool();
            else
                tool.startingTool = false;
        }

        protected override void OnClick()
        {
            base.OnClick();
            
            if (toolManager.CurrentTool == tool) return;
            
            SelectTool();
        }

        private void SelectTool()
        {
            toolManager.SelectTool(tool);
            image.color = activatedColor;
            image.sprite = pushedButtonSprite;
            Dialogue.DeselectToolButton(this);
        }

        public void DeselectButton()
        {
            image.color = Color.white;
            image.sprite = unpushedButtonSprite;
        }
    }
}
using Audio;
using Managers;
using ToolSystem;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Cleaning
{
    [RequireComponent(typeof(Button))]
    public class SelectToolButton : DialogueComponent<CleaningDialogue>
    {
        [SerializeField] private Tool tool;
        [SerializeField] private Color activatedColor;
        [SerializeField] private Sprite unpushedButtonSprite;
        [SerializeField] private Sprite pushedButtonSprite;
        
        private ToolManager toolManager;
        
        private Image image;
        private Button dialogueButton;
        private PlayOneShot audioComp;

        protected override void OnComponentAwake()
        {
            TryGetComponent(out dialogueButton);
            TryGetComponent(out audioComp);
            
            if (!tool || !tool.unlocked) dialogueButton.interactable = false;
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

        protected override void Subscribe()
        {
            dialogueButton.onClick.AddListener(OnSubmit);
        }

        protected override void Unsubscribe()
        {
            dialogueButton.onClick.RemoveListener(OnSubmit);
        }

        private void OnSubmit()
        {
            if (toolManager.CurrentTool == tool) return;
            
            SelectTool();
            audioComp.PlayOnce();
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
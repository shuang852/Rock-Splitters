using UI.Core;
using UI.Generic;

namespace UI
{
    public class ToolsDialogue : Dialogue
    {
        private SelectToolButton activeButton;
        protected override void OnClose()
        {
        }
    
        protected override void OnPromote()
        {
        }
    
        protected override void OnDemote()
        {
        }

        public void DeselectToolButton(SelectToolButton selectToolButton)
        {
            if (activeButton)
                activeButton.DeselectButton();
            
            activeButton = selectToolButton;
        }
        
    }
}
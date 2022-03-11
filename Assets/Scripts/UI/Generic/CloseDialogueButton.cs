using UI.Core;

namespace UI.Generic
{
    public class CloseDialogueButton : DialogueButton<Dialogue>
    {
        protected override void OnClick()
        {
            base.OnClick();
            
            Manager.Pop();
        }
    }
}

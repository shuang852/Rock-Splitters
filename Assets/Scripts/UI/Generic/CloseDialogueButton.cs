using UI.Core;

namespace UI.Generic
{
    public class CloseDialogueButton : DialogueButton<Dialogue>
    {
        protected override void Subscribe() { }
        protected override void Unsubscribe() { }

        protected override void OnClick()
        {
            // BUG: Pop is internal.
            // Manager.Pop();
        }
    }
}

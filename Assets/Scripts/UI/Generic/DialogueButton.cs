using UnityEngine;
using UnityEngine.UI;
using UI.Core;

namespace UI.Generic
{
    [RequireComponent(typeof(Button))]
    public abstract class DialogueButton<T> : DialogueComponent<T> where T : Dialogue
    {
        private Button button;

        protected override void OnComponentAwake()
        {
            TryGetComponent(out button);
        }

        protected override void Subscribe()
        {
            button.onClick.AddListener(OnClick);
        }

        protected override void Unsubscribe()
        {
            button.onClick.RemoveListener(OnClick);
        }

        protected abstract void OnClick();
    }
}

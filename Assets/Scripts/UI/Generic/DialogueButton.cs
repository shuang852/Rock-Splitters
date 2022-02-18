using Audio;
using UnityEngine;
using UnityEngine.UI;
using UI.Core;

namespace UI.Generic
{
    [RequireComponent(typeof(Button), typeof(PlayOneShot))]
    public abstract class DialogueButton<T> : DialogueComponent<T> where T : Dialogue
    {
        protected Button button;
        private PlayOneShot audioComp;

        protected override void OnComponentAwake()
        {
            TryGetComponent(out button);
            TryGetComponent(out audioComp);
        }

        protected override void Subscribe()
        {
            button.onClick.AddListener(OnClick);
        }

        protected override void Unsubscribe()
        {
            button.onClick.RemoveListener(OnClick);
        }

        protected virtual void OnClick()
        {
            audioComp.PlayOnce();
        }
    }
}

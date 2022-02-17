using System;
using Audio;
using UI.Core;
using UnityEngine;

namespace UI.Generic
{
    public class CloseDialogueButton : DialogueButton<Dialogue>
    {
        [SerializeField, HideInInspector] private PlayOneShot audioComp;
        
        protected override void OnClick()
        {
            audioComp.PlayOnce();
            Manager.Pop();
        }

        private void OnValidate()
        {
            TryGetComponent(out audioComp);
        }
    }
}

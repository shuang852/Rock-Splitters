using System;
using Audio;
using UI.Core;
using UI.Generic;
using UnityEngine;

namespace UI.Pause
{
    public class OpenDialogueButton : DialogueButton<Dialogue>
    {
        [SerializeField] private GameObject dialoguePrefab;
        [SerializeField, HideInInspector] private PlayOneShot audioComp;

        protected override void OnClick()
        {
            audioComp.PlayOnce();
            Instantiate(dialoguePrefab);
        }

        private void OnValidate()
        {
            TryGetComponent(out audioComp);
        }
    }
}

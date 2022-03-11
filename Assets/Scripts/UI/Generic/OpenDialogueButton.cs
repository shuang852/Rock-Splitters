using UI.Core;
using UI.Generic;
using UnityEngine;

namespace UI.Pause
{
    public class OpenDialogueButton : DialogueButton<Dialogue>
    {
        [SerializeField] private GameObject dialoguePrefab;

        protected override void OnClick()
        {
            base.OnClick();
            
            Instantiate(dialoguePrefab);
        }
    }
}

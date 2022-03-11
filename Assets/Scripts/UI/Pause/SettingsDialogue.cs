using UI.Core;
using UnityEngine;

namespace UI.Pause
{
    public class SettingsDialogue : Dialogue
    {
        [SerializeField] private GameObject blurBackground;
        protected override void OnClose()
        {
        }

        protected override void OnPromote()
        {
            if (!blurBackground.activeSelf) blurBackground.SetActive(true);
        }

        protected override void OnDemote()
        {
            blurBackground.SetActive(false);
        }
    }
}
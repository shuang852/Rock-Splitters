using Effects;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Pause
{
    public class CameraShakeToggle : DialogueComponent<SettingsDialogue>
    {
        private CameraShake cameraShake;
        private Toggle toggle;

        protected override void OnComponentStart()
        {
            if (Camera.main != null) cameraShake = Camera.main.GetComponent<CameraShake>();
            toggle = GetComponent<Toggle>();
            toggle.isOn = cameraShake.ShakeEnabled;
            toggle.onValueChanged.AddListener(OnToggle);
        }

        private void OnToggle(bool value)
        {
            cameraShake.ShakeEnabled = value;
        }

        protected override void Subscribe()
        {
        }

        protected override void Unsubscribe()
        {
        }
    }
}
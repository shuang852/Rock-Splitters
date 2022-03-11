using System;
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
            if (Camera.main != null) Camera.main.TryGetComponent(out cameraShake);
            TryGetComponent(out toggle);
            LoadSettings();
            toggle.onValueChanged.AddListener(OnToggle);
        }

        private void OnToggle(bool value)
        {
            if (cameraShake) cameraShake.ShakeEnabled = value;

        }

        protected override void Subscribe()
        {
        }

        protected override void Unsubscribe()
        {
        }

        private void OnDestroy()
        {
            SaveSettings();
            if (cameraShake) 
                cameraShake.ShakeEnabled = toggle.isOn;
        }

        private void SaveSettings()
        {
            PlayerPrefs.SetInt(CameraShake.PrefName, toggle.isOn ? 1 : 0);
        }
        
        private void LoadSettings()
        {
            if (!PlayerPrefs.HasKey(CameraShake.PrefName)) return;

            toggle.isOn = PlayerPrefs.GetInt(CameraShake.PrefName) == 1;
        }
    }
}
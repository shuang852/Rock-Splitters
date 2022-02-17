using Audio;
using UI.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Pause
{
    public class AbandonButton : DialogueButton<PauseDialogue>
    {
        [SerializeField, HideInInspector] private PlayOneShot audioComp;
        [SerializeField] private int mainMenuIndex;

        protected override void OnClick()
        {
            audioComp.PlayOnce();
            Dialogue.Abandoned?.Invoke();
            SceneManager.LoadSceneAsync(mainMenuIndex);
        }
        
        private void OnValidate()
        {
            TryGetComponent(out audioComp);
        }
    }
}

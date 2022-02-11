using UI.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Pause
{
    public class AbandonButton : DialogueButton<PauseDialogue>
    {
        [SerializeField] private int mainMenuIndex;

        protected override void OnClick()
        {
            Dialogue.Abandoned?.Invoke();
            SceneManager.LoadSceneAsync(mainMenuIndex);
        }
    }
}

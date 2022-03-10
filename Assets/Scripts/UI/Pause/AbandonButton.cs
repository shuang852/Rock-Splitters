using Managers;
using UI.Generic;
using UI.Transitions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Pause
{
    public class AbandonButton : DialogueButton<PauseDialogue>
    {
        [SerializeField] private SceneReference mainMenuSceneReference;
        private SceneTransitionManager sceneTransitionManager;
        protected override void OnComponentStart()
        {
            base.OnComponentStart();
            sceneTransitionManager = M.GetOrThrow<SceneTransitionManager>();
        }

        protected override void OnClick()
        {
            base.OnClick();
            
            Dialogue.Abandoned?.Invoke();
            sceneTransitionManager.TransitionOut(mainMenuSceneReference);
        }
    }
}

using Managers;
using UI.Core;
using UI.Transitions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Generic
{
    [RequireComponent(typeof(Button))]
    public class LoadSceneButton : DialogueButton<Dialogue>
    {
        [SerializeField] private SceneReference sceneReference;
        private SceneTransitionManager sceneTransitionManager;

        private static bool loadingInProgress;

        protected override void OnComponentStart()
        {
            base.OnComponentStart();
            sceneTransitionManager = M.GetOrThrow<SceneTransitionManager>();
        }

        protected override void OnClick()
        {
            base.OnClick();
            
            if (loadingInProgress)
            {
                Debug.LogError("Tried loading scene while another was already being loaded!");
                return;
            }
                
            Debug.Log($"Loading Scene '{sceneReference.ScenePath}'.");
            loadingInProgress = true;
            
            sceneTransitionManager.TransitionOut(sceneReference);
        }
        
        private void OnDestroy()
        {
            loadingInProgress = false;
        }
    }
}

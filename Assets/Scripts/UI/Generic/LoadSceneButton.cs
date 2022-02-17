using Audio;
using UI.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Generic
{
    [RequireComponent(typeof(Button))]
    public class LoadSceneButton : DialogueComponent<Dialogue>
    {
        [SerializeField] private SceneReference sceneReference;
        [SerializeField, HideInInspector] private PlayOneShot audioComp;

        private Button dialogueButton;

        private static bool loadingInProgress;
        
        protected override void OnComponentAwake()
        {
            TryGetComponent(out dialogueButton);
        }

        protected override void Subscribe()
        {
            dialogueButton.onClick.AddListener(OnSubmit);
        }

        protected override void Unsubscribe()
        {
            dialogueButton.onClick.RemoveListener(OnSubmit);
        }

        private void OnSubmit()
        {
            if (loadingInProgress)
            {
                Debug.LogError("Tried loading scene while another was already being loaded!");
                return;
            }
                
            Debug.Log($"Loading Scene '{sceneReference.ScenePath}'.");
            audioComp.PlayOnce();
            loadingInProgress = true;
            
            SceneManager.LoadSceneAsync(sceneReference);
        }
        
        private void OnDestroy()
        {
            loadingInProgress = false;
        }
        
        private void OnValidate()
        {
            TryGetComponent(out audioComp);
        }
    }
}

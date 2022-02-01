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
        
        private Button button;

        private static bool loadingInProgress;
        
        protected override void OnComponentAwake()
        {
            TryGetComponent(out button);
            button.onClick.AddListener(OnSubmit);
        }
        
        protected override void Subscribe() { }
        
        protected override void Unsubscribe() { }

        private void OnSubmit()
        {
            if (loadingInProgress)
            {
                Debug.LogError("Tried loading scene while another was already being loaded!");
                return;
            }
                
            Debug.Log($"Loading Scene '{sceneReference.ScenePath}'.");

            loadingInProgress = true;
            
            SceneManager.LoadSceneAsync(sceneReference);
        }
        
        private void OnDestroy()
        {
            loadingInProgress = false;
        }
    }
}

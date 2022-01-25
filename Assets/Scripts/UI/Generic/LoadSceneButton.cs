using UI.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Generic
{
    [RequireComponent(typeof(Button))]
    public class LoadSceneButton : DialogueComponent<Dialogue>
    {
        [SerializeField] private int sceneIndexInBuild;
        
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
                
            Scene scene = SceneManager.GetSceneByBuildIndex(sceneIndexInBuild);
            Debug.Log($"Loading Scene '{scene.name}'.");

            loadingInProgress = true;
            
            SceneManager.LoadSceneAsync(sceneIndexInBuild);
        }
        
        private void OnDestroy()
        {
            loadingInProgress = false;
        }
    }
}

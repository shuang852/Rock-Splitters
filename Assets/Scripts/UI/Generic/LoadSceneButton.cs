using System.Collections;
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

        private static Coroutine currentCoroutine;
        
        protected override void OnComponentAwake()
        {
            TryGetComponent(out button);
            button.onClick.AddListener(OnSubmit);
        }
        
        protected override void Subscribe() { }
        
        protected override void Unsubscribe() { }

        private void OnSubmit()
        {
            if (currentCoroutine != null)
            {
                Debug.LogError("Tried loading scene while another was already being loaded!");
                return;
            }
                
            Scene scene = SceneManager.GetSceneByBuildIndex(sceneIndexInBuild);
            Debug.Log($"Loading Scene '{scene.name}'.");
            currentCoroutine = StartCoroutine(LoadSceneAsync(scene));
        }

        private IEnumerator LoadSceneAsync(Scene scene)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndexInBuild);
            while (!operation.isDone)
                yield return null;

            currentCoroutine = null;
        }
    }
}

using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Transitions
{
    public class SceneTransitionManager : Manager
    {
        public override bool PersistBetweenScenes => true;

        [SerializeField] private float transitionDuration = 1f;
        [Tooltip("Sets the transitions ease mode")]
        [SerializeField] private Ease easeMode;
        [SerializeField, HideInInspector] private CanvasGroup canvasGroup;

        private void TransitionIn(Scene scene, LoadSceneMode loadSceneMode)
        {
            canvasGroup.DOFade(0, transitionDuration)
                .SetEase(easeMode);
        }

        /// <summary>
        /// Call when transitioning out of a scene. Runs a transition then loads. 
        /// </summary>
        /// <param name="sceneReference">Scene to load to</param>
        public async void TransitionOut(SceneReference sceneReference)
        {
            await canvasGroup.DOFade(1, transitionDuration)
                .SetEase(easeMode)
                .AsyncWaitForCompletion();
            await SceneManager.LoadSceneAsync(sceneReference);
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();
            
            if (!canvasGroup) 
                canvasGroup = GetComponent<CanvasGroup>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SceneManager.sceneLoaded += TransitionIn;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SceneManager.sceneLoaded -= TransitionIn;
        }
    }
}
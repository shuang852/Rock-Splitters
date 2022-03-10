using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI.Transitions
{
    public class SceneTransitionManager : Manager
    {
        public override bool PersistBetweenScenes => true;

        [SerializeField] private float transitionDuration = 1f;
        [Tooltip("Sets the transitions ease mode")]
        [SerializeField] private Ease easeMode;
        [SerializeField, HideInInspector] private CanvasGroup canvasGroup;
        [SerializeField] private Image animatedDino;

        /// <summary>
        /// Call when transitioning into a scene. Runs a transition then disables itself. 
        /// </summary>
        private async void TransitionIn(Scene scene, LoadSceneMode loadSceneMode)
        {;
            await canvasGroup.DOFade(0, transitionDuration)
                .SetEase(easeMode)
                .AsyncWaitForCompletion();
            animatedDino.gameObject.SetActive(false);
            canvasGroup.blocksRaycasts = false;
        }

        /// <summary>
        /// Call when transitioning out of a scene. Runs a transition then loads. 
        /// </summary>
        /// <param name="sceneReference">Scene to load to</param>
        public async void TransitionOut(SceneReference sceneReference)
        {
            animatedDino.gameObject.SetActive(true);
            await canvasGroup.DOFade(1, transitionDuration)
                .SetEase(easeMode)
                .AsyncWaitForCompletion();
            canvasGroup.blocksRaycasts = true;
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
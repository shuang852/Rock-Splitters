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
        [SerializeField, HideInInspector] private CanvasGroup canvasGroup;
        [SerializeField] private Ease easeMode;
        
        private void TransitionIn(Scene scene, LoadSceneMode loadSceneMode)
        {
            canvasGroup.DOFade(0, transitionDuration)
                .SetEase(easeMode);
        }

        public async void TransitionOut(SceneReference sceneReference)
        {
            await canvasGroup.DOFade(1, transitionDuration)
                .SetEase(easeMode)
                .AsyncWaitForCompletion();
            SceneManager.LoadSceneAsync(sceneReference);
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
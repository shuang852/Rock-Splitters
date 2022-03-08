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
        [SerializeField] private Sprite[] sprites;
        [SerializeField] private Image image;
        [SerializeField] private float minSize = 0.01f;
        [SerializeField] private float maxSize = 10f;

        private async void TransitionIn(Scene scene, LoadSceneMode loadSceneMode)
        {
            image.sprite = sprites[Random.Range(0, sprites.Length - 1)];
            //transform.localScale = new Vector3()
            await image.transform.DOScale(new Vector3(minSize, minSize, 1), transitionDuration)
                .SetEase(easeMode)
                .AsyncWaitForCompletion();
            image.gameObject.SetActive(false);
            canvasGroup.blocksRaycasts = false;
            // canvasGroup.DOFade(0, transitionDuration)
            //     .SetEase(easeMode);
        }

        /// <summary>
        /// Call when transitioning out of a scene. Runs a transition then loads. 
        /// </summary>
        /// <param name="sceneReference">Scene to load to</param>
        public async void TransitionOut(SceneReference sceneReference)
        {
            // await canvasGroup.DOFade(1, transitionDuration)
            //     .SetEase(easeMode)
            //     .AsyncWaitForCompletion();
            image.sprite = sprites[Random.Range(0, sprites.Length - 1)];
            image.gameObject.SetActive(true);
            canvasGroup.blocksRaycasts = true;
            await image.transform.DOScale(new Vector3(maxSize, maxSize, 1), transitionDuration)
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
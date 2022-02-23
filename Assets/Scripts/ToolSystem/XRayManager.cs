using System.Collections;
using Managers;
using RockSystem.Artefacts;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ToolSystem
{
    public class XRayManager : Manager
    {
        [FormerlySerializedAs("fossilSprite")] [SerializeField] private SpriteRenderer artefactSpriteRenderer;
        [FormerlySerializedAs("fossilFadeCurve")] [SerializeField] private AnimationCurve artefactFadeCurve;
        [FormerlySerializedAs("fossilFadeDuration")] [SerializeField] private float artefactFadeDuration;
        [SerializeField] private Image flashImage;
        [SerializeField] private AnimationCurve flashFadeCurve;
        [SerializeField] private float flashFadeDuration;

        private Coroutine coroutine;

        private ArtefactShapeManager artefactShapeManager;

        protected override void Start()
        {
            base.Start();

            artefactShapeManager = M.GetOrThrow<ArtefactShapeManager>();
            
            artefactShapeManager.initialised.AddListener(OnArtefactShapeInitialised);
        }

        private void OnArtefactShapeInitialised()
        {
            Initialise();
        }

        // TODO: Update to support multiple ArtefactShapes
        private void Initialise()
        {
            var artefactSpriteTransform = artefactSpriteRenderer.transform;
            var artefactShapeTransform = artefactShapeManager.MainArtefactShape.transform;
            
            // TODO: May need to rework how these are found and set.
            artefactSpriteRenderer.sprite = artefactShapeManager.MainArtefactShape.Artefact.Sprite;
            artefactSpriteTransform.position = artefactShapeTransform.position;
            artefactSpriteTransform.rotation = artefactShapeTransform.rotation;
            artefactSpriteTransform.localScale = artefactShapeTransform.localScale;
        }

        public void ShowXRay()
        {
            artefactSpriteRenderer.color = Color.white;
            flashImage.color = Color.white;

            if (coroutine != null)
                StopCoroutine(coroutine);
            
            coroutine = StartCoroutine(XRayFade());
        }

        private IEnumerator XRayFade()
        {
            float time = 0;

            while (time < Mathf.Max(artefactFadeDuration, flashFadeDuration))
            {
                if (time < artefactFadeDuration)
                {
                    var artefactColor = artefactSpriteRenderer.color;

                    artefactSpriteRenderer.color = new Color(
                        artefactColor.r,
                        artefactColor.r,
                        artefactColor.r,
                        artefactFadeCurve.Evaluate(time / artefactFadeDuration)
                    );
                }

                if (time < flashFadeDuration)
                {
                    var flashColor = flashImage.color;

                    flashImage.color = new Color(
                        flashColor.r,
                        flashColor.r,
                        flashColor.r,
                        flashFadeCurve.Evaluate(time / flashFadeDuration)
                    );
                }

                yield return null;

                time += Time.deltaTime;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            artefactShapeManager.initialised.RemoveListener(OnArtefactShapeInitialised);
        }
    }
}
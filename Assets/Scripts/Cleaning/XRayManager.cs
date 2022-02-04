using System.Collections;
using Managers;
using RockSystem.Fossils;
using UnityEngine;
using UnityEngine.UI;

namespace Cleaning
{
    public class XRayManager : Manager
    {
        [SerializeField] private SpriteRenderer fossilSprite;
        [SerializeField] private AnimationCurve fossilFadeCurve;
        [SerializeField] private float fossilFadeDuration;
        [SerializeField] private Image flashImage;
        [SerializeField] private AnimationCurve flashFadeCurve;
        [SerializeField] private float flashFadeDuration;

        private Coroutine coroutine;

        private FossilShape fossilShape;

        protected override void Start()
        {
            base.Start();

            fossilShape = M.GetOrThrow<FossilShape>();
            
            var fossilSpriteTransform = fossilSprite.transform;
            var fossilShapeTransform = fossilShape.transform;
            
            // TODO: May need to rework how these are found and set.
            fossilSprite.sprite = fossilShape.Antiquity.Sprite;
            fossilSpriteTransform.position = fossilShapeTransform.position;
            fossilSpriteTransform.rotation = fossilShapeTransform.rotation;
            fossilSpriteTransform.localScale = fossilShapeTransform.localScale;
        }

        public void ShowXRay()
        {
            fossilSprite.color = Color.white;
            flashImage.color = Color.white;

            if (coroutine != null)
                StopCoroutine(coroutine);
            
            coroutine = StartCoroutine(XRayFade());
        }

        private IEnumerator XRayFade()
        {
            float time = 0;

            while (time < Mathf.Max(fossilFadeDuration, flashFadeDuration))
            {
                if (time < fossilFadeDuration)
                {
                    var fossilColor = fossilSprite.color;

                    fossilSprite.color = new Color(
                        fossilColor.r,
                        fossilColor.r,
                        fossilColor.r,
                        fossilFadeCurve.Evaluate(time / fossilFadeDuration)
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
    }
}
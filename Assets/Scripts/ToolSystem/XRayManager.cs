using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using RockSystem.Chunks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ToolSystem
{
    public class XRayManager : Manager
    {
        [FormerlySerializedAs("artefactFadeCurve")]
        [FormerlySerializedAs("fossilFadeCurve")]
        [SerializeField] private AnimationCurve chunkShapeFadeCurve;
        [FormerlySerializedAs("artefactFadeDuration")]
        [FormerlySerializedAs("fossilFadeDuration")]
        [SerializeField] private float chunkShapeFadeDuration;
        [SerializeField] private Image flashImage;
        [SerializeField] private AnimationCurve flashFadeCurve;
        [SerializeField] private float flashFadeDuration;
        [SerializeField] private string sortingLayer;

        private Coroutine coroutine;

        private ChunkManager chunkManager;

        private readonly Dictionary<ChunkShape, GameObject> gameObjects = new Dictionary<ChunkShape, GameObject>();
        private readonly Dictionary<ChunkShape, SpriteRenderer> spriteRenderers = new Dictionary<ChunkShape, SpriteRenderer>();

        protected override void Start()
        {
            base.Start();

            chunkManager = M.GetOrThrow<ChunkManager>();
            
            chunkManager.chunkShapeRegistered.AddListener(OnChunkShapeRegistered);
            chunkManager.chunkShapeUnregistered.AddListener(OnChunkShapeUnregistered);
        }

        private void OnChunkShapeRegistered(ChunkShape chunkShape)
        {
            GameObject go = new GameObject("X-Ray ChunkShape");
            go.transform.parent = transform;
            gameObjects.Add(chunkShape, go);
            SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
            spriteRenderers.Add(chunkShape, spriteRenderer);

            spriteRenderer.sprite = chunkShape.Sprite;
            spriteRenderer.sortingLayerName = sortingLayer;
            spriteRenderer.sortingOrder = chunkShape.Layer;

            var spriteTransform = go.transform;
            var shapeTransform = chunkShape.transform;

            spriteTransform.position = shapeTransform.position;
            spriteTransform.rotation = shapeTransform.rotation;
            spriteTransform.localScale = shapeTransform.lossyScale;
            
            SetOpacity(spriteRenderer, 0);
        }

        private void OnChunkShapeUnregistered(ChunkShape chunkShape)
        {
            if (!gameObjects.ContainsKey(chunkShape))
                throw new ArgumentException($"The GameObject for {nameof(ChunkShape)} {chunkShape} could not be found.");
            
            if (!spriteRenderers.ContainsKey(chunkShape))
                throw new ArgumentException($"The SpriteRenderer for {nameof(ChunkShape)} {chunkShape} could not be found.");
            
            Destroy(gameObjects[chunkShape]);

            gameObjects.Remove(chunkShape);
            spriteRenderers.Remove(chunkShape);
        }

        public void ShowXRay()
        {
            SetSpriteRendererOpacities(1);

            flashImage.color = Color.white;

            if (coroutine != null)
                StopCoroutine(coroutine);
            
            coroutine = StartCoroutine(XRayFade());
        }

        private IEnumerator XRayFade()
        {
            float time = 0;

            while (time < Mathf.Max(chunkShapeFadeDuration, flashFadeDuration))
            {
                if (time < chunkShapeFadeDuration)
                    SetSpriteRendererOpacities(chunkShapeFadeCurve.Evaluate(time / chunkShapeFadeDuration));

                if (time < flashFadeDuration)
                    SetOpacity(flashImage, flashFadeCurve.Evaluate(time / flashFadeDuration));

                yield return null;

                time += Time.deltaTime;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            chunkManager.chunkShapeRegistered.RemoveListener(OnChunkShapeRegistered);
            chunkManager.chunkShapeUnregistered.RemoveListener(OnChunkShapeUnregistered);
        }

        private void SetSpriteRendererOpacities(float opacity)
        {
            foreach (var spriteRenderer in spriteRenderers.Values)
            {
                SetOpacity(spriteRenderer, opacity);
            }
        }

        private void SetOpacity(SpriteRenderer spriteRenderer, float opacity)
        {
            var color = spriteRenderer.color;

            spriteRenderer.color = new Color(
                color.r,
                color.r,
                color.r,
                opacity
            );
        }

        private void SetOpacity(Image image, float opacity)
        {
            var color = image.color;

            image.color = new Color(
                color.r,
                color.r,
                color.r,
                opacity
            );
        }
    }
}
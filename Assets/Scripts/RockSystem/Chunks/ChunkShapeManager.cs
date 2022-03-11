using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace RockSystem.Chunks
{
    public class ChunkShapeManager<T> : Manager where T : ChunkShape
    {
        [FormerlySerializedAs("minePrefab")] [SerializeField] protected GameObject chunkShapePrefab;
        
        public bool ChunkShapesCanBeDamaged
        {
            get => chunkShapesCanBeDamaged;
            set
            {
                ChunkShapes.ForEach(a => a.CanBeDamaged = value);
                
                chunkShapesCanBeDamaged = value;
            }
        }
        
        private bool chunkShapesCanBeDamaged = true;
        protected readonly List<T> ChunkShapes = new List<T>();

        protected readonly Dictionary<ChunkShape, GameObject> ChunkShapeGameObjects =
            new Dictionary<ChunkShape, GameObject>();
        protected ChunkManager ChunkManager;
        private readonly List<ChunkShape> destroyRequests = new List<ChunkShape>();

        protected override void Start()
        {
            base.Start();
            
            ChunkManager = M.GetOrThrow<ChunkManager>();
        }

        protected virtual void Initialise()
        {
            Deinitialise();
        }
        
        protected virtual void Deinitialise()
        {
            while (ChunkShapes.Count > 0)
            {
                DestroyChunkShape(ChunkShapes.First());
            }
        }

        protected virtual T CreateChunkShape(Func<GameObject> instantiationFunction, Action<ChunkShape> preinitialisationAction, Action<T> initialisationAction)
        {
            var go = instantiationFunction();

            var chunkShape = go.GetComponent<T>();
            
            ChunkShapeGameObjects.Add(chunkShape, go);
            


            preinitialisationAction(chunkShape);

            initialisationAction(chunkShape);
            
            ChunkShapes.Add(chunkShape);
            
            // TODO: Should this just be handled by the ChunkShape? Leads to two way dependency
            // TODO: Has been moved before initialisation to allow mines defused during initialisation to be removed from the xray
            // TODO: Moved back because now artefacts don't have xrays
            ChunkManager.RegisterChunkShape(chunkShape);

            chunkShape.CanBeDamaged = ChunkShapesCanBeDamaged;
            chunkShape.destroyRequest.AddListener(OnChunkShapeDestroyRequest);
            chunkShape.destroyed.AddListener(OnChunkShapeDestroyed);

            return chunkShape;
        }

        private void OnChunkShapeDestroyRequest(ChunkShape chunkShape)
        {
            destroyRequests.Add(chunkShape);
        }

        private void OnChunkShapeDestroyed(ChunkShape chunkShape)
        {
            if (!ChunkShapes.Contains(chunkShape)) return;
            
            Debug.LogWarning($"{nameof(ChunkShape)}s should be destroyed by a {nameof(ChunkShapeManager<ChunkShape>)}. " +
                             $"Allowing {nameof(ChunkShape)}s to destroy themselves can cause problems.");
                
            DestroyChunkShape((T) chunkShape);
        }

        protected virtual void DestroyChunkShape(T chunkShape)
        {
            ChunkShapes.Remove(chunkShape);

            ChunkManager.UnregisterChunkShape(chunkShape);
            
            chunkShape.destroyed.RemoveListener(OnChunkShapeDestroyed);
            
            Destroy(ChunkShapeGameObjects[chunkShape]);

            ChunkShapeGameObjects.Remove(chunkShape);
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            Deinitialise();
        }
        
        public void CheckHealthAndExposure()
        {
            bool healthChanged = false;
            bool exposureChanged = false;
            
            foreach (var chunkShape in ChunkShapes)
            {
                if (chunkShape.CheckHealth()) healthChanged = true;

                if (chunkShape.CheckExposure()) exposureChanged = true;
            }
            
            if (healthChanged)
                OnHealthChanged();

            if (exposureChanged)
                OnExposureChanged();
            
            HandleDestroyRequests();
        }
        
        protected virtual void OnHealthChanged() {}
        
        protected virtual void OnExposureChanged() {}

        private void HandleDestroyRequests()
        {
            while (destroyRequests.Count > 0)
            {
                // Debug.Log($"chunk shape destroyed {destroyRequests[0].name}");
                DestroyChunkShape((T) destroyRequests[0]);
                
                destroyRequests.RemoveAt(0);
            }
        }
    }
}
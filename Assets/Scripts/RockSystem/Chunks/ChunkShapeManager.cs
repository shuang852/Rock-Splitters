using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;

namespace RockSystem.Chunks
{
    public class ChunkShapeManager<T> : Manager where T : ChunkShape
    {
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
        protected readonly List<GameObject> ChunkShapeGameObjects = new List<GameObject>();
        protected ChunkManager ChunkManager;

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
                UnregisterChunkShape(ChunkShapes.First());
            }

            ChunkShapeGameObjects.ForEach(Destroy);
            
            ChunkShapes.Clear();
            ChunkShapeGameObjects.Clear();
        }

        protected virtual void RegisterChunkShape(T chunkShape)
        {
            ChunkShapes.Add(chunkShape);
            
            // TODO: Should this just be handled by the ChunkShape? Leads to two way dependency
            ChunkManager.RegisterChunkShape(chunkShape);

            chunkShape.CanBeDamaged = ChunkShapesCanBeDamaged;
            chunkShape.destroyed.AddListener(OnChunkShapeDestroyed);
        }

        private void OnChunkShapeDestroyed(ChunkShape chunkShape)
        {
            UnregisterChunkShape((T) chunkShape);
        }

        protected virtual void UnregisterChunkShape(T chunkShape)
        {
            ChunkShapes.Remove(chunkShape);

            ChunkManager.UnregisterChunkShape(chunkShape);
            
            chunkShape.destroyed.RemoveListener(OnChunkShapeDestroyed);
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
        }
        
        protected virtual void OnHealthChanged() {}
        
        protected virtual void OnExposureChanged() {}
    }
}
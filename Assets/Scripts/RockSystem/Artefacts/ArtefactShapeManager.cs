using System.Collections.Generic;
using System.Linq;
using RockSystem.Chunks;
using Stored;
using UnityEngine;
using UnityEngine.Events;
using Utility;

namespace RockSystem.Artefacts
{
    public class ArtefactShapeManager : ChunkShapeManager<ArtefactShape>
    {
        public UnityEvent initialised = new UnityEvent();
        public UnityEvent artefactDamaged = new UnityEvent();
        public UnityEvent artefactExposed = new UnityEvent();
        public UnityEvent<ArtefactShape, Vector2Int> artefactChunkDamaged = new UnityEvent<ArtefactShape, Vector2Int>();

        // TODO: Try to avoid using this whereever possible, it should be switched out for a list of main artefacts
        public ArtefactShape MainArtefactShape => artefacts.FirstOrDefault();
        
        // TODO: Health and exposure can be updated to only include certain "main" artefacts
        public float Exposure => (float) artefacts.Sum(a => a.ExposedChunks) / artefacts.Sum(a => a.NumOfChunks);
        public float Health => artefacts.Sum(a => a.CurrentTotalHealth) / artefacts.Sum(a => a.MaxTotalHealth);

        private List<ArtefactShape> artefacts => ChunkShapes;
        
        // TODO: Should support being passed multiple Artefacts
        public void Initialise(Artefact artefact)
        {
            base.Initialise();
            
            GameObject go = new GameObject(artefact.DisplayName);
            go.transform.parent = transform;
            ChunkShapeGameObjects.Add(go);
            ArtefactShape artefactShape = go.AddComponent<ArtefactShape>();

            artefactShape.Initialise(artefact);
            
            RegisterArtefact(artefactShape);

            initialised.Invoke();
        }

        private void OnArtefactShapeChunkDamaged(ChunkShape chunkShape, Vector2Int flatPosition)
        {
            artefactChunkDamaged.Invoke((ArtefactShape) chunkShape, flatPosition);
        }

        protected override void RegisterArtefact(ArtefactShape artefactShape)
        {
            base.RegisterArtefact(artefactShape);
            
            artefactShape.chunkDamaged.AddListener(OnArtefactShapeChunkDamaged);
        }

        protected override void UnregisterArtefact(ArtefactShape artefactShape)
        {
            base.UnregisterArtefact(artefactShape);
            
            artefactShape.chunkDamaged.RemoveListener(OnArtefactShapeChunkDamaged);
        }

        public ArtefactShape GetExposedArtefactAtFlatPosition(Hexagons.OddrChunkCoord oddrChunkCoord)
        {
            Chunk chunk = ChunkManager.ChunkStructure.GetOrNull(oddrChunkCoord);

            if (chunk == null)
                return artefacts.Find(f => f.IsHitAtFlatPosition(oddrChunkCoord));

            return null;
        }

        protected override void OnHealthChanged()
        {
            base.OnHealthChanged();
            
            artefactDamaged.Invoke();
        }

        protected override void OnExposureChanged()
        {
            base.OnExposureChanged();
            
            artefactExposed.Invoke();
        }
    }
}
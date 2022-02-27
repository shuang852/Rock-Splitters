using System;
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
        public ArtefactShape MainArtefactShape => Artefacts.FirstOrDefault();
        
        // TODO: Health and exposure can be updated to only include certain "main" artefacts
        public float Exposure => (float) Artefacts.Sum(a => a.ExposedChunks) / Artefacts.Sum(a => a.NumOfChunks);
        public float Health => Artefacts.Sum(a => a.CurrentTotalHealth) / Artefacts.Sum(a => a.MaxTotalHealth);

        private List<ArtefactShape> Artefacts => ChunkShapes;
        
        // TODO: Should support being passed multiple Artefacts
        public void Initialise(Artefact artefact)
        {
            base.Initialise();
            
            CreateChunkShape(
                () => Instantiate(chunkShapePrefab, transform),
                artefactShape => artefactShape.Initialise(artefact)
            );

            initialised.Invoke();
        }

        private void OnArtefactShapeChunkDamaged(ChunkShape chunkShape, Vector2Int flatPosition)
        {
            artefactChunkDamaged.Invoke((ArtefactShape) chunkShape, flatPosition);
        }

        protected override ArtefactShape CreateChunkShape(Func<GameObject> instantiationFunction, Action<ArtefactShape> initialisationAction)
        {
            var artefactShape = base.CreateChunkShape(instantiationFunction, initialisationAction);
            
            artefactShape.chunkDamaged.AddListener(OnArtefactShapeChunkDamaged);

            return artefactShape;
        }

        protected override void DestroyChunkShape(ArtefactShape artefactShape)
        {
            base.DestroyChunkShape(artefactShape);
            
            artefactShape.chunkDamaged.RemoveListener(OnArtefactShapeChunkDamaged);
        }

        public ArtefactShape GetExposedArtefactAtFlatPosition(Hexagons.OddrChunkCoord oddrChunkCoord)
        {
            Chunk chunk = ChunkManager.ChunkStructure.GetOrNull(oddrChunkCoord);

            if (chunk == null)
                return Artefacts.Find(f => f.IsHitAtFlatPosition(oddrChunkCoord));

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
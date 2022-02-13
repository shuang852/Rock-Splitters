using System.Collections.Generic;
using Managers;
using RockSystem.Chunks;
using UnityEngine;
using UnityEngine.Events;
using Utility;

namespace RockSystem.Artefacts
{
    public class ArtefactShapeManager : Manager
    {
        public UnityEvent<ArtefactShape, Vector2Int> artefactDamaged = new UnityEvent<ArtefactShape, Vector2Int>();
        
        private ChunkManager chunkManager;
        
        private readonly List<ArtefactShape> artefacts = new List<ArtefactShape>();
        
        protected override void Start()
        {
            base.Start();

            chunkManager = M.GetOrThrow<ChunkManager>();

            chunkManager.damageOverflow.AddListener(OnDamageOverflow);
        }

        private void OnDamageOverflow(Vector2Int flatPosition, float damage)
        {
            ArtefactShape artefact = GetExposedArtefactAtFlatPosition(flatPosition);

            if (artefact == null) return;
            
            artefact.DamageArtefactChunk(flatPosition, damage);
            
            artefactDamaged.Invoke(artefact, flatPosition);
        }
        
        internal void RegisterArtefact(ArtefactShape artefact)
        {
            artefacts.Add(artefact);
        }

        internal void UnregisterArtefact(ArtefactShape artefact)
        {
            artefacts.Remove(artefact);
        }

        public ArtefactShape GetExposedArtefactAtFlatPosition(Hexagons.OddrChunkCoord oddrChunkCoord)
        {
            Chunk chunk = chunkManager.ChunkStructure.GetOrNull(oddrChunkCoord);

            if (chunk == null)
                return artefacts.Find(f => f.IsHitAtFlatPosition(oddrChunkCoord));

            return null;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            chunkManager.damageOverflow.RemoveListener(OnDamageOverflow);
        }
    }
}
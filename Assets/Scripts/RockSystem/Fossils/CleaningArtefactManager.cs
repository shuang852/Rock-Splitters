using System.Collections.Generic;
using Managers;
using RockSystem.Chunks;
using UnityEngine;
using UnityEngine.Events;
using Utility;

namespace RockSystem.Fossils
{
    public class CleaningArtefactManager : Manager
    {
        public UnityEvent<FossilShape, Vector2Int> fossilDamaged = new UnityEvent<FossilShape, Vector2Int>();
        
        private ChunkManager chunkManager;
        
        private readonly List<FossilShape> fossils = new List<FossilShape>();
        
        protected override void Start()
        {
            base.Start();

            chunkManager = M.GetOrThrow<ChunkManager>();

            chunkManager.damageOverflow.AddListener(OnDamageOverflow);
        }

        private void OnDamageOverflow(Vector2Int flatPosition, float damage)
        {
            FossilShape fossil = GetExposedFossilAtFlatPosition(flatPosition);

            if (fossil == null) return;
            
            fossil.DamageFossilChunk(flatPosition, damage);
            
            fossilDamaged.Invoke(fossil, flatPosition);
        }
        
        internal void RegisterFossil(FossilShape fossil)
        {
            fossils.Add(fossil);
        }

        public FossilShape GetExposedFossilAtFlatPosition(Hexagons.OddrChunkCoord oddrChunkCoord)
        {
            Chunk chunk = chunkManager.chunkStructure.GetOrNull(oddrChunkCoord);

            if (chunk == null)
                return fossils.Find(f => f.IsHitAtFlatPosition(oddrChunkCoord));

            return null;
        }
    }
}
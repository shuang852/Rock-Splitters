using System.Collections.Generic;
using Managers;
using RockSystem.Chunks;
using UnityEngine;
using Utility;

namespace RockSystem.Fossils
{
    public class ArtefactManager : Manager
    {
        private DamageLayer damageLayer;
        private ChunkManager chunkManager;
        
        private List<FossilShape> fossils = new List<FossilShape>();
        
        protected override void Start()
        {
            base.Start();

            damageLayer = M.GetOrThrow<DamageLayer>();
            chunkManager = M.GetOrThrow<ChunkManager>();

            chunkManager.damageOverflow.AddListener(OnDamageOverflow);
        }

        private void OnDamageOverflow(Vector2Int flatPosition, float damage)
        {
            FossilShape fossil = GetExposedFossilAtFlatPosition(flatPosition);

            if (fossil == null) return;
            
            fossil.DamageFossilChunk(flatPosition, damage);
            
            float remainingHealth = fossil.GetFossilChunkHealth(flatPosition);
            
            if (remainingHealth <= fossil.Antiquity.BreakingHealth)
            {
                float damagePercentage = 1f - (remainingHealth / fossil.Antiquity.MaxHealth);
                damageLayer.DisplayDamage(flatPosition, damagePercentage);
            }
        }
        
        internal void RegisterFossil(FossilShape fossil)
        {
            fossils.Add(fossil);
        }
        
        // TODO: This function is never used, now I'm confused
        public FossilShape GetFossilAtFlatPosition(Hexagons.OddrChunkCoord oddrChunkCoord)
        {
            Chunk chunk = chunkManager.chunkStructure.GetOrNull(oddrChunkCoord);

            if (chunk == null) return fossils.Find(f => f.IsHitAtFlatPosition(oddrChunkCoord));
                
            return GetFossilAtPosition(chunk.Position);
        }
        
        public FossilShape GetExposedFossilAtFlatPosition(Hexagons.OddrChunkCoord oddrChunkCoord)
        {
            Chunk chunk = chunkManager.chunkStructure.GetOrNull(oddrChunkCoord);

            if (chunk == null)
                return fossils.Find(f => f.IsHitAtFlatPosition(oddrChunkCoord));
            
            return GetFossilAtPosition(chunk.Position);
        }

        private FossilShape GetFossilAtPosition(Vector3Int position) =>
            fossils.Find(f => f.IsHitAtPosition(position));
        
        public bool WillDamageRock(List<Hexagons.OddrChunkCoord> oddrChunkCoords)
        {
            foreach (var flatPosition in oddrChunkCoords)
            {
                if (GetExposedFossilAtFlatPosition(flatPosition) != null) continue;
                
                if (chunkManager.chunkStructure.GetOrNull(flatPosition) == null) continue;

                return true;
            }

            return false;
        }

    }
}
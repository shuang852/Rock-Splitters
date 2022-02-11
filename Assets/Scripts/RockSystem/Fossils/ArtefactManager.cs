using System.Collections.Generic;
using Managers;
using RockSystem.Chunks;
using UnityEngine;
using Utility;

namespace RockSystem.Fossils
{
    public class ArtefactManager : Manager
    {
        // TODO: Invert dependency.
        private DamageLayer damageLayer;
        private ChunkManager chunkManager;
        
        private readonly List<FossilShape> fossils = new List<FossilShape>();
        
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

        public FossilShape GetExposedFossilAtFlatPosition(Hexagons.OddrChunkCoord oddrChunkCoord)
        {
            Chunk chunk = chunkManager.chunkStructure.GetOrNull(oddrChunkCoord);

            if (chunk == null)
                return fossils.Find(f => f.IsHitAtFlatPosition(oddrChunkCoord));

            return null;
        }
    }
}
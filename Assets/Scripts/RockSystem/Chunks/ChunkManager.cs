using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using RockSystem.Fossils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using OddrChunkCoord = Utility.Hexagons.OddrChunkCoord;

namespace RockSystem.Chunks
{
    /// <summary>
    /// Handles the implementation, functionality and access of chunks.
    /// </summary>
    [RequireComponent(typeof(ChunkMap))]
    public class ChunkManager : Manager
    {
        [SerializeField] private Vector3Int size = new Vector3Int(40, 90, 6);
        [SerializeField] private List<ChunkDescription> rocks;

        private ChunkMap chunkMap;
        internal ChunkStructure chunkStructure;
        public Grid CurrentGrid { get; private set; }
        private DamageLayer damageLayer;

        // TODO: ChunkManager should not also handle fossils. Maybe create ArtefactRockManager?
        private List<FossilShape> fossils = new List<FossilShape>();

        public UnityEvent<Chunk> chunkCleared = new UnityEvent<Chunk>();

        protected override void Awake()
        {
            base.Awake();
            chunkMap = GetComponent<ChunkMap>();
            CurrentGrid = GetComponent<Grid>();
            damageLayer = GetComponent<DamageLayer>();

            if (rocks.Count < 1)
                throw new InvalidOperationException($"No rocks assigned to the {nameof(ChunkManager)}!");
            
            chunkMap.LayerLength = size.z;
            chunkMap.CreateTilemaps();

            chunkStructure = new ChunkStructure(
                size,
                CurrentGrid, 
                () => PickRandomFromList(rocks),
                ChunkSetBehaviour,
                ChunkClearBehaviour,
                flatPosition => flatPosition.x < 0 
            );
        }

        public void DamageChunk(Vector2 worldPosition, float damage)
        {
            OddrChunkCoord flatPosition = chunkStructure.WorldToCell(worldPosition);
            DamageChunk(flatPosition, damage);
        }

        // TODO: Should be renamed to convey that it damages multiple chunks in a column.
        public void DamageChunk(OddrChunkCoord flatPosition, float damage, bool willDamageFossil = true)
        {
            while (damage > 0)
            {
                FossilShape fossil = GetFossilAtPosition(flatPosition);

                if (fossil != null)
                {
                    if (!willDamageFossil) break;
                    
                    fossil.DamageFossilChunk(flatPosition, damage);
                    float remainingHealth = fossil.GetFossilChunkHealth(flatPosition);

                    if (remainingHealth <= fossil.Antiquity.BreakingHealth)
                    {
                        float damagePercentage = 1f - (remainingHealth / fossil.Antiquity.MaxHealth);
                        damageLayer.DisplayDamage(flatPosition, damagePercentage);
                    }

                    break;
                }
                
                Chunk chunk = chunkStructure.GetOrNull(flatPosition);

                if (chunk == null) break;
                
                float damageTaken = chunk.DamageChunk(damage);
                damage -= damageTaken;
            }
        }

        public FossilShape GetFossilAtPosition(OddrChunkCoord oddrChunkCoord)
        {
            Chunk chunk = chunkStructure.GetOrNull(oddrChunkCoord);

            if (chunk == null) return fossils.Find(f => f.IsHitAtFlatPosition(oddrChunkCoord));
                
            return GetFossilAtPosition(chunk.Position);
        }

        private FossilShape GetFossilAtPosition(Vector3Int position) =>
            fossils.Find(f => f.IsHitAtPosition(position));

        internal void RegisterFossil(FossilShape fossil)
        {
            fossils.Add(fossil);
        }

        private T PickRandomFromList<T>(List<T> list) => 
            list[UnityEngine.Random.Range(0, list.Count - 1)];

        public Vector2 GetChunkWorldPosition(OddrChunkCoord oddrChunkCoord)
        {
            return CurrentGrid.CellToWorld(oddrChunkCoord);
        }

        public bool WillDamageRock(List<OddrChunkCoord> oddrChunkCoords)
        {
            foreach (var flatPosition in oddrChunkCoords)
            {
                if (GetFossilAtPosition(flatPosition) != null) continue;
                
                if (chunkStructure.GetOrNull(flatPosition) == null) continue;

                return true;
            }

            return false;
        }

        private void ChunkSetBehaviour(Chunk chunk)
        {
            TileBase tile = chunk.CreateTile();
            
            chunkMap.SetTileAtLayer(chunk.Position, tile);
        }

        private void ChunkClearBehaviour(Chunk chunk)
        {
            chunkMap.ClearTileAtLayer(chunk.Position);
            
            chunkCleared.Invoke(chunk);
        }

        public void HideRock()
        {
            chunkMap.HideRock();
        }
    }
}
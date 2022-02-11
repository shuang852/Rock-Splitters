using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using Utility;
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
        [SerializeField] private RockShapeMask rockShapeMask;

        private ChunkMap chunkMap;
        internal ChunkStructure chunkStructure;
        public Grid CurrentGrid { get; private set; }

        public UnityEvent<Vector2Int, float> damageOverflow = new UnityEvent<Vector2Int, float>();
        public UnityEvent<Chunk> chunkCleared = new UnityEvent<Chunk>();
        
        public RockShape RockShape { get; private set; }
        public Color RockColor { get; private set; }
        public ChunkDescription ChunkDescription { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            
            chunkMap = GetComponent<ChunkMap>();
            CurrentGrid = GetComponent<Grid>();
        }

        public void Initialise(RockShape rockShape, Color rockColor, ChunkDescription chunkDescription)
        {
            RockShape = rockShape;
            RockColor = rockColor;
            ChunkDescription = chunkDescription;
            
            Initialise();
        }

        public void Initialise()
        {
            rockShapeMask.Initialise(RockShape);
            
            chunkMap.Initialise(RockColor, size.z);

            chunkStructure = new ChunkStructure(
                size,
                CurrentGrid, 
                ChunkDescription,
                ChunkSetBehaviour,
                ChunkClearBehaviour,
                flatPosition =>
                    Hexagons.HexagonOverlapsCollider(CurrentGrid, flatPosition, rockShapeMask.PolyCollider)
            );
        }

        public void DamageChunk(Vector2 worldPosition, float damage)
        {
            OddrChunkCoord flatPosition = chunkStructure.WorldToCell(worldPosition);
            DamageChunk(flatPosition, damage);
        }

        // TODO: Should be renamed to convey that it damages multiple chunks in a column.
        public void DamageChunk(OddrChunkCoord flatPosition, float damage, bool damageWillOverflow = true)
        {
            while (damage > 0)
            {
                Chunk chunk = chunkStructure.GetOrNull(flatPosition);

                if (chunk == null)
                {
                    if (damageWillOverflow) damageOverflow.Invoke(flatPosition, damage);
                    break;
                }
                
                float damageTaken = chunk.DamageChunk(damage);
                damage -= damageTaken;
            }
        }

        public Vector2 GetChunkWorldPosition(OddrChunkCoord oddrChunkCoord)
        {
            return CurrentGrid.CellToWorld(oddrChunkCoord);
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
        
        public bool WillDamageRock(List<OddrChunkCoord> oddrChunkCoords)
        {
            return oddrChunkCoords.Any(flatPosition => chunkStructure.GetOrNull(flatPosition) != null);
        }
    }
}
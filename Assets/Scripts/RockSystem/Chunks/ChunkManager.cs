using System;
using System.Collections.Generic;
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
        [SerializeField] private List<ChunkDescription> rocks;
        [SerializeField] private RockShapeMask rockShapeMask;

        private ChunkMap chunkMap;
        internal ChunkStructure chunkStructure;
        public Grid CurrentGrid { get; private set; }

        public UnityEvent<Vector2Int, float> damageOverflow = new UnityEvent<Vector2Int, float>();
        public UnityEvent<Chunk> chunkCleared = new UnityEvent<Chunk>();

        protected override void Awake()
        {
            base.Awake();
            chunkMap = GetComponent<ChunkMap>();
            CurrentGrid = GetComponent<Grid>();

            if (rocks.Count < 1)
                throw new InvalidOperationException($"No rocks assigned to the {nameof(ChunkManager)}!");

            rockShapeMask.Setup();
            
            chunkMap.LayerLength = size.z;
            chunkMap.CreateTilemaps();

            chunkStructure = new ChunkStructure(
                size,
                CurrentGrid, 
                () => PickRandomFromList(rocks),
                ChunkSetBehaviour,
                ChunkClearBehaviour,
                flatPosition =>
                    Hexagons.HexagonOverlapsCollider(CurrentGrid, flatPosition, rockShapeMask.PolyCollider)
            );
            
            // TODO: Update fossil exposure
            // fossils.ForEach(f => f.ForceUpdateFossilExposure());
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

        private T PickRandomFromList<T>(List<T> list) => 
            list[UnityEngine.Random.Range(0, list.Count - 1)];

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
    }
}
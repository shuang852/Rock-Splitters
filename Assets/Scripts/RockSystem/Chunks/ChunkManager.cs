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
        internal ChunkStructure ChunkStructure;
        public Grid CurrentGrid { get; private set; }

        public UnityEvent<Vector2Int, float> damageOverflow = new UnityEvent<Vector2Int, float>();
        public UnityEvent<Chunk> chunkCleared = new UnityEvent<Chunk>();
        public UnityEvent<ChunkShape> chunkShapeRegistered = new UnityEvent<ChunkShape>();
        public UnityEvent<ChunkShape> chunkShapeUnregistered = new UnityEvent<ChunkShape>();
        
        public RockShape RockShape { get; private set; }
        public Color RockColor { get; private set; }
        public ChunkDescription ChunkDescription { get; private set; }

        private readonly List<ChunkShape> chunkShapes = new List<ChunkShape>();

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

            ChunkStructure = new ChunkStructure(
                size,
                CurrentGrid, 
                ChunkDescription,
                ChunkSetBehaviour,
                ChunkClearBehaviour,
                flatPosition =>
                    Hexagons.HexagonOverlapsCollider(CurrentGrid, flatPosition, rockShapeMask.PolyCollider)
            );
        }

        public void DamageChunksAtPosition(Vector2 worldPosition, float damage)
        {
            OddrChunkCoord flatPosition = ChunkStructure.WorldToCell(worldPosition);
            DamageChunksAtPosition(flatPosition, damage);
        }

        public void DamageChunksAtPosition(OddrChunkCoord flatPosition, float damage)
        {
            while (damage > 0)
            {
                ChunkShape chunkShape = GetExposedChunkShape(flatPosition);

                if (chunkShape != null)
                {
                    chunkShape.DamageChunk(flatPosition, damage);
                    break;
                }

                Chunk chunk = ChunkStructure.GetOrNull(flatPosition);

                if (chunk == null)
                {
                    // TODO: No longer used. Will we use this in the future?
                    damageOverflow.Invoke(flatPosition, damage);
                    break;
                }
                
                float damageTaken = chunk.DamageChunk(damage);
                damage -= damageTaken;
            }
        }

        // BUG: If the chunks below a fossil get removed it makes the fossil impossible to damage.
        private ChunkShape GetExposedChunkShape(OddrChunkCoord flatPosition)
        {
            Chunk chunk = ChunkStructure.GetOrNull(flatPosition);

            // A ChunkShape is only exposed if it is above the top chunk layer
            int zPos = chunk == null ? 0 : chunk.Position.z + 1;

            return GetChunkShape(new Vector3Int(flatPosition.col, flatPosition.row, zPos));
            
            // TODO: Is it more effcient to just use chunkShape.IsExposedAtFlatPosition?
        }

        private ChunkShape GetChunkShape(Vector3Int position)
        {
            return chunkShapes.Find(f => f.IsHitAtPosition(position));
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
            return oddrChunkCoords.Any(flatPosition => ChunkStructure.GetOrNull(flatPosition) != null);
        }

        public void RegisterChunkShape(ChunkShape chunkShape)
        {
            chunkShapes.Add(chunkShape);
            
            chunkShapeRegistered.Invoke(chunkShape);
        }
        
        public void UnregisterChunkShape(ChunkShape chunkShape)
        {
            chunkShapes.Remove(chunkShape);
            
            chunkShapeUnregistered.Invoke(chunkShape);
        }
    }
}
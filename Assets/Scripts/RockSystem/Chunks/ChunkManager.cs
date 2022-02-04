using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using RockSystem.Fossils;
using UnityEngine;

namespace RockSystem.Chunks
{
    /// <summary>
    /// Handles the implementation, functionality and access of chunks.
    /// </summary>
    [RequireComponent(typeof(ChunkMap))]
    public class ChunkManager : Manager
    {
        [SerializeField] private Vector2Int size = new Vector2Int(40, 90);
        [SerializeField] private List<ChunkDescription> rocks;

        private ChunkMap chunkMap;
        internal ChunkStructure chunkStructure;
        private Grid grid;
        private DamageLayer damageLayer;

        // TODO: ChunkManager should not also handle fossils. Maybe create ArtefactRockManager?
        private List<FossilShape> fossils = new List<FossilShape>();

        public struct OddrChunkCoord
        {
            public OddrChunkCoord(int col, int row)
            {
                coord = new Vector2Int(col, row);
            }
            
            public Vector2Int coord;
            
            public int col => coord.x;
            public int row => coord.y;
            
            public static implicit operator OddrChunkCoord(Vector2Int v) => new OddrChunkCoord(v.x, v.y);
            public static implicit operator Vector2Int(OddrChunkCoord o) => new Vector2Int(o.col, o.row);
            
            public static implicit operator OddrChunkCoord(Vector3Int v) => new OddrChunkCoord(v.x, v.y);
            public static implicit operator Vector3Int(OddrChunkCoord o) => new Vector3Int(o.col, o.row, 0);
        }

        public struct AxialChunkCoord
        {
            public AxialChunkCoord(int q, int r)
            {
                coord = new Vector2Int(q, r);
            }
            
            public Vector2Int coord;

            public int q => coord.x;
            public int r => coord.y;
            
            public static AxialChunkCoord operator +(AxialChunkCoord a, AxialChunkCoord b) => new AxialChunkCoord(a.q + b.q, a.r + b.r);
        }

        protected override void Awake()
        {
            base.Awake();
            chunkMap = GetComponent<ChunkMap>();
            grid = GetComponent<Grid>();
            damageLayer = GetComponent<DamageLayer>();

            if (rocks.Count < 1)
                throw new InvalidOperationException($"No rocks assigned to the {nameof(ChunkManager)}!");

            chunkStructure = new ChunkStructure(size, chunkMap, grid, () => PickRandomFromList(rocks));
        }

        public void DamageChunk(Vector2 worldPosition, float damage)
        {
            OddrChunkCoord flatPosition = chunkStructure.WorldToCell(worldPosition);
            DamageChunk(flatPosition, damage);
        }

        public void DamageChunk(OddrChunkCoord flatPosition, float damage, bool willDamageFossil = true)
        {
            while (damage > 0)
            {
                Chunk chunk = chunkStructure.GetOrNull(flatPosition);

                if (chunk == null) break;
                
                FossilShape fossil = GetFossilAtPosition(chunk.Position);

                if (fossil == null)
                {
                    float damageTaken = chunk.DamageChunk(damage);
                    damage -= damageTaken;
                }
                else
                {
                    if (!willDamageFossil) break;
                    
                    fossil.DamageFossilChunk(chunk.FlatPosition, damage);
                    float remainingHealth = fossil.GetFossilChunkHealth(chunk.FlatPosition);

                    if (remainingHealth <= fossil.Antiquity.BreakingHealth)
                    {
                        float damagePercentage = 1f - (remainingHealth / fossil.Antiquity.MaxHealth);
                        damageLayer.DisplayDamage(chunk.FlatPosition, damagePercentage);
                    }

                    break;
                }
            }
        }

        public FossilShape GetFossilAtPosition(OddrChunkCoord oddrChunkCoord)
        {
            Chunk chunk = chunkStructure.GetOrNull(oddrChunkCoord);

            if (chunk == null) return null;
                
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
        
        /// <summary>
        /// Returns all the chunks inside a circle with the given centre and radius.
        /// </summary>
        /// <param name="centreWorldPosition"></param>
        /// <param name="radius"></param>
        /// <returns>A list of odd-r chunk coordinates.</returns>
        public List<OddrChunkCoord> GetChunksInRadius(Vector2 centreWorldPosition, float radius)
        {
            var chunksInRange = GetChunksInEnclosingRange(centreWorldPosition, radius);

            List<OddrChunkCoord> chunksInRadius = new List<OddrChunkCoord>();

            foreach (var chunk in chunksInRange)
            {
                Vector2 chunkWorldPosition = grid.CellToWorld(chunk);
                
                float distanceToPointer = Vector2.Distance(chunkWorldPosition, centreWorldPosition);

                if (distanceToPointer < radius)
                    chunksInRadius.Add(chunk);
            }

            return chunksInRadius;
        }

        /// <summary>
        /// Returns all the chunks in a range that encloses the given radius. Makes searching for chunks more efficient.
        /// </summary>
        /// <param name="centreWorldPosition"></param>
        /// <param name="radius"></param>
        /// <returns>A list of odd-r chunk coordinates.</returns>
        private List<OddrChunkCoord> GetChunksInEnclosingRange(Vector2 centreWorldPosition, float radius)
        {
            OddrChunkCoord oddrChunkCoord = grid.WorldToCell(centreWorldPosition);

            AxialChunkCoord axialChunkCoord = OddrToAxial(oddrChunkCoord);

            // The outer radius (measured centre to corner) of the hexagon that fully encloses the given radius
            float outerHexRadius = (float)(radius * 2 / Math.Sqrt(3));
            
            int outerHexRange = Mathf.CeilToInt(outerHexRadius / grid.cellSize.x);

            List<AxialChunkCoord> axialChunksInRange = GetChunksInRange(axialChunkCoord, outerHexRange);

            return axialChunksInRange.Select(AxialToOddr).ToList();
        }

        /// <summary>
        /// Returns all the chunks that are no more than x adjacent moves from the given world position.
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <param name="range"></param>
        /// <returns>A list of odd-r chunk coordinates.</returns>
        public List<OddrChunkCoord> GetChunksInRange(Vector2 worldPosition, int range)
        {
            OddrChunkCoord oddrChunkCoord = grid.WorldToCell(worldPosition);

            AxialChunkCoord axialChunkCoord = OddrToAxial(oddrChunkCoord);
            
            List<AxialChunkCoord> axialChunksInRange = GetChunksInRange(axialChunkCoord, range);

            return axialChunksInRange.Select(AxialToOddr).ToList();
        }

        /// <summary>
        /// Returns all the chunks that are no more than x adjacent moves from the given chunk.
        /// </summary>
        /// <param name="axialChunkCoord"></param>
        /// <param name="range"></param>
        /// <returns>A list of axial chunk coordinates.</returns>
        public List<AxialChunkCoord> GetChunksInRange(AxialChunkCoord axialChunkCoord, int range)
        {
            List<AxialChunkCoord> chunks = new List<AxialChunkCoord>();

            for (int q = -range; q <= range; q++)
            {
                var lowerBound = Mathf.Max(-range, -q - range);
                var upperBound = Mathf.Min(range, -q + range);
                
                for (int r = lowerBound; r <= upperBound; r++)
                {
                    var chunkOffset = new AxialChunkCoord(q, r);
                    
                    chunks.Add(axialChunkCoord + chunkOffset);
                }
            }

            return chunks;
        }

        private AxialChunkCoord OddrToAxial(OddrChunkCoord oddrChunkCoord)
        {
            var q = oddrChunkCoord.col - (oddrChunkCoord.row - (oddrChunkCoord.row & 1)) / 2;
            var r = oddrChunkCoord.row;
            return new AxialChunkCoord(q, r);
        }

        private OddrChunkCoord AxialToOddr(AxialChunkCoord axialChunkCoord)
        {
            var col = axialChunkCoord.q + (axialChunkCoord.r - (axialChunkCoord.r & 1)) / 2;
            var row = axialChunkCoord.r;
            return new OddrChunkCoord(col, row);
        }

        public Vector2 GetChunkWorldPosition(OddrChunkCoord oddrChunkCoord)
        {
            return grid.CellToWorld(oddrChunkCoord);
        }

        public bool WillDamageRock(List<OddrChunkCoord> oddrChunkCoords)
        {
            return oddrChunkCoords.Select(GetFossilAtPosition).Any(fossil => fossil == null);
        }
    }
}
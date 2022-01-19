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

        private List<FossilShape> fossils = new List<FossilShape>();

        protected override void Awake()
        {
            base.Awake();
            chunkMap = GetComponent<ChunkMap>();
            grid = GetComponent<Grid>();
            damageLayer = GetComponent<DamageLayer>();

            if (rocks.Count < 1)
                throw new InvalidOperationException($"No rocks assigned to the {nameof(ChunkManager)}!");

            chunkStructure = new ChunkStructure(size, chunkMap, grid);
        }

        protected override void Start()
        {
            base.Start();
            
            GenerateChunks();
        }
        
        public void DamageChunk(Vector2 worldPosition, int damage = 1)
        {
            Vector2Int flatPosition = chunkStructure.WorldToCell(worldPosition);
            DamageChunk(flatPosition, damage);
        }

        public void DamageChunk(Vector2Int flatPosition, int damage = 1)
        {
            Chunk chunk = chunkStructure.GetOrNull(flatPosition);

            if (chunk != null)
            {
                FossilShape fossil = GetFossilAtPosition(chunk.Position);

                if (fossil == null)
                {
                    chunk.DamageChunk(damage);
                }
                else
                {
                    damageLayer.DisplayDamage(chunk.FlatPosition);
                }
            }
        }

        private FossilShape GetFossilAtPosition(Vector3Int position) =>
            fossils.Find(f => f.IsHitAtPosition(position));

        internal void RegisterFossil(FossilShape fossil)
        {
            fossils.Add(fossil);
        }

        private void GenerateChunks()
        {
            // Generate random rocks
            foreach (Vector3Int position in chunkStructure.GetPositions())
            {
                ChunkDescription rockDescription = PickRandomFromList(rocks);
                chunkStructure.Set(new Chunk(rockDescription, new Vector3Int(position.x, position.y, position.z)));
            }
        }

        private T PickRandomFromList<T>(List<T> list) => 
            list[UnityEngine.Random.Range(0, list.Count - 1)];
        
        /// <summary>
        /// Returns all the chunks inside a circle with the given centre and radius.
        /// </summary>
        /// <param name="centreWorldPosition"></param>
        /// <param name="radius"></param>
        /// <returns>A list of odd-r chunk coordinates.</returns>
        public List<Vector2Int> GetChunksInRadius(Vector2 centreWorldPosition, float radius)
        {
            // var chunksInRange = GetChunksInEnclosingRange(centreWorldPosition, radius);

            var chunksInRange = GetChunksInRange(centreWorldPosition, 50);

            List<Vector2Int> chunksInRadius = new List<Vector2Int>();

            foreach (var chunk in chunksInRange)
            {
                Vector2 chunkWorldPosition = grid.CellToWorld((Vector3Int)chunk);
                
                float distanceToPointer = Vector2.Distance(chunkWorldPosition, centreWorldPosition);

                if (distanceToPointer < radius)
                    chunksInRadius.Add(chunk);
            }

            return chunksInRadius;
        }

        // TODO: Not currently working.
        /// <summary>
        /// Returns all the chunks in a range that encloses the given radius. Makes searching for chunks more efficient.
        /// </summary>
        /// <param name="centreWorldPosition"></param>
        /// <param name="radius"></param>
        /// <returns>A list of odd-r chunk coordinates.</returns>
        private List<Vector2Int> GetChunksInEnclosingRange(Vector2 centreWorldPosition, float radius)
        {
            Vector2Int nearestChunk = (Vector2Int)grid.WorldToCell(centreWorldPosition);

            // The outer radius (measured centre to corner) of the hexagon that fully encloses the given radius
            float outerHexRadius = (float)(radius * 2 / Math.Sqrt(3));
            
            int outerHexRange = Mathf.CeilToInt(outerHexRadius / grid.cellSize.x);

            List<Vector2Int> axialChunksInRange = GetChunksInRange(nearestChunk, outerHexRange);

            return axialChunksInRange.Select(AxialToOddr).ToList();
        }

        /// <summary>
        /// Returns all the chunks that are no more than x adjacent moves from the given world position.
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <param name="range"></param>
        /// <returns>A list of odd-r chunk coordinates.</returns>
        public List<Vector2Int> GetChunksInRange(Vector2 worldPosition, int range)
        {
            Vector2Int oddrChunkCoord = (Vector2Int) grid.WorldToCell(worldPosition);

            Vector2Int axialChunkCoord = OddrToAxial(oddrChunkCoord);
            
            List<Vector2Int> axialChunksInRange = GetChunksInRange(axialChunkCoord, range);

            return axialChunksInRange.Select(AxialToOddr).ToList();
        }

        /// <summary>
        /// Returns all the chunks that are no more than x adjacent moves from the given chunk.
        /// </summary>
        /// <param name="axialChunkCoord"></param>
        /// <param name="range"></param>
        /// <returns>A list of axial chunk coordinates.</returns>
        public List<Vector2Int> GetChunksInRange(Vector2Int axialChunkCoord, int range)
        {
            List<Vector2Int> chunks = new List<Vector2Int>();

            for (int x = -range; x <= range; x++)
            {
                var lowerBound = Mathf.Max(-range, -x - range);
                var upperBound = Mathf.Min(range, -x + range);
                
                for (int y = lowerBound; y <= upperBound; y++)
                {
                    var chunkOffset = new Vector2Int(x, y);
                    
                    chunks.Add(axialChunkCoord + chunkOffset);
                }
            }

            return chunks;
        }

        private Vector2Int OddrToAxial(Vector2Int oddrChunkCoord)
        {
            var q = oddrChunkCoord.x - (oddrChunkCoord.y - (oddrChunkCoord.y & 1)) / 2;
            var r = oddrChunkCoord.y;
            return new Vector2Int(q, r);
        }

        private Vector2Int AxialToOddr(Vector2Int axialChunkCoord)
        {
            var col = axialChunkCoord.x + (axialChunkCoord.y - (axialChunkCoord.y & 1)) / 2;
            var row = axialChunkCoord.y;
            return new Vector2Int(col, row);
        }
    }
}
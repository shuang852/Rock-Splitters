using System;
using System.Collections.Generic;
using UnityEngine;

namespace RockSystem.Chunks
{
    /// <summary>
    /// Provides a way to manipulate, control and contain chunks.
    /// </summary>
    internal class ChunkStructure
    {
        private readonly Dictionary<Vector2Int, LinkedList<Chunk>> chunks =
            new Dictionary<Vector2Int, LinkedList<Chunk>>();
        private readonly Grid grid;
        private readonly ChunkDescription chunkDescription;
        private readonly Action<Chunk> chunkSetBehaviour;
        private readonly Action<Chunk> chunkClearBehaviour;
        private readonly Func<Vector2Int, bool> rockShapeMask;


        public Vector2Int MinSize { get; }
        public Vector2Int MaxSize { get; }

        public int MaxDepth { get; }
        // Flip x and y, so that it aligns with the proper world coordinate system. Remember that in the tilemap it's already inverted.
        public Vector2 CellSize => new Vector2(grid.cellSize.y, grid.cellSize.x);
        public IEnumerable<Vector2Int> FlatPositions { get; }
        public IEnumerable<Vector3Int> Positions { get; }

        internal ChunkStructure(Vector3Int size, Grid grid, ChunkDescription chunkDescription, Action<Chunk>chunkSetBehaviour, Action<Chunk> chunkClearBehaviour, Func<Vector2Int, bool> rockShapeMask)
        {
            this.grid = grid;
            this.chunkDescription = chunkDescription;
            this.chunkSetBehaviour = chunkSetBehaviour;
            this.chunkClearBehaviour = chunk => 
            {
                Clear(chunk.FlatPosition);

                chunkClearBehaviour(chunk);
            };
            this.rockShapeMask = rockShapeMask;

            MinSize = new Vector2Int(Mathf.FloorToInt(size.x / -2f), Mathf.FloorToInt(size.y / -2f));
            MaxSize = new Vector2Int(Mathf.FloorToInt(size.x / 2f), Mathf.FloorToInt(size.y / 2f));
            MaxDepth = size.z;

            FlatPositions = GetFlatPositions();
            Positions = GetPositions();

            // Initialise the LinkedLists
            foreach (var flatPosition in FlatPositions)
            {
                chunks[flatPosition] = new LinkedList<Chunk>();

                if (!rockShapeMask(flatPosition)) continue;

                for (int i = 0; i < MaxDepth; i++)
                {
                    Set(new Vector3Int(flatPosition.x, flatPosition.y, i));
                }
            }
        }

        private void Set(Vector3Int position)
        {
            var chunk = new Chunk(
                chunkDescription,
                position,
                chunkClearBehaviour
            );

            chunks[chunk.FlatPosition].AddLast(chunk);

            chunkSetBehaviour(chunk);
        }

        private void Clear(Vector2Int flatPosition)
        {
            if (!IsInBounds(flatPosition)) return;
            
            chunks[flatPosition].RemoveLast();
        }
        
        // TODO: Update for new chunks data structure.
        // public void Clear(Vector3Int position)
        // {
        //     if (IsInBounds(position))
        //     {
        //         chunks.Remove(position);
        //         chunkMap.ClearTileAtLayer(position);
        //     }
        // }

        private bool IsInBounds(Vector2Int flatPosition) => flatPosition.x >= MinSize.x && flatPosition.y >= MinSize.y
            && flatPosition.x <= MaxSize.x && flatPosition.y <= MaxSize.y;
        
        private bool IsInBounds(Vector3Int position) => position.x >= MinSize.x && position.y >= MinSize.y
            && position.x <= MaxSize.y && position.y <= MaxSize.y 
            && position.z >= 0 && position.z < MaxDepth;

        public Chunk GetOrNull(Vector2Int flatPosition)
        {
            if (!IsInBounds(flatPosition)) return null;
            
            return chunks[flatPosition].Last?.Value;
        }
        
        // TODO: Update for new chunks data structure.
        // public bool TryGet(Vector3Int position, out Chunk chunk)
        // {
        //     return chunks.TryGetValue(position, out chunk);
        // } 

        // public Chunk GetOrNull(Vector3Int position) => 
        //     TryGet(position, out Chunk chunk) ? chunk : null;

        public Vector3 CellToWorld(Vector2Int cell) => grid.GetCellCenterWorld((Vector3Int) cell);
        
        public Vector2Int WorldToCell(Vector3 worldPosition) => (Vector2Int) grid.WorldToCell(worldPosition);

        private IEnumerable<Vector2Int> GetFlatPositions()
        {
            for (int i = MinSize.x; i <= MaxSize.x; i++)
            {
                for (int j = MinSize.y; j <= MaxSize.y; j++)
                {
                    yield return new Vector2Int(i, j);
                }
            }
        }

        private IEnumerable<Vector3Int> GetPositions()
        {
            for (int k = 0; k <= MaxDepth; k++)
            {
                for (int i = MinSize.x; i <= MaxSize.x; i++)
                {
                    for (int j = MinSize.y; j <= MaxSize.y; j++)
                    {
                        yield return new Vector3Int(i, j, k);
                    }
                }
            }
        }
    }
}
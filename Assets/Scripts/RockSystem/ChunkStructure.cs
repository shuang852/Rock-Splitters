using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RockSystem
{
    /// <summary>
    /// Provides a way to manipulate, control and contain chunks.
    /// </summary>
    internal class ChunkStructure
    {
        private readonly Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();
        private readonly ChunkMap chunkMap;

        public Vector2Int MinSize { get; }
        public Vector2Int MaxSize { get; }

        internal ChunkStructure(Vector2Int size, ChunkMap chunkMap)
        {
            this.chunkMap = chunkMap;

            MinSize = new Vector2Int(Mathf.FloorToInt(size.x / -2f), Mathf.FloorToInt(size.y / -2f));
            MaxSize = new Vector2Int(Mathf.FloorToInt(size.x / 2f), Mathf.FloorToInt(size.y / 2f));
        }

        public void Set(Chunk chunk)
        {
            if (IsInBounds(chunk.Position))
            {
                chunk.AttachTo(this);
                chunks[chunk.Position] = chunk;
                TileBase tile = chunk.CreateTile();
                chunkMap.SetTileAtLayer(chunk.Position, tile);
            }
        }

        public void Clear(Vector3Int position)
        {
            if (IsInBounds(position))
            {
                chunks.Remove(position);
                chunkMap.ClearTileAtLayer(position);
            }
        }

        public bool HasChunk(Vector3Int position) => chunks.ContainsKey(position);

        private bool IsInBounds(Vector3Int position) => position.x >= MinSize.x && position.y >= MinSize.y
            && position.x <= MaxSize.y && position.y <= MaxSize.y 
            && position.z >= 0 && position.z < chunkMap.LayerLength;

        public bool TryGet(Vector3Int position, out Chunk chunk)
        {
            return chunks.TryGetValue(position, out chunk);
        } 

        public Chunk GetOrNull(Vector3Int position) => 
            TryGet(position, out Chunk chunk) ? chunk : null;

        public Chunk GetOrNull(Vector2Int flatPosition)
        {
            for (int i = chunkMap.LayerLength; i >= 0; i--)
            {
                Vector3Int position = new Vector3Int(flatPosition.x, flatPosition.y, i);

                if (TryGet(position, out Chunk chunk))
                    return chunk;
            }

            return null;
        }

        public IEnumerable<Vector2Int> GetFlatPositions()
        {
            for (int i = MinSize.x; i <= MaxSize.x; i++)
            {
                for (int j = MinSize.y; j <= MaxSize.y; j++)
                {
                    yield return new Vector2Int(i, j);
                }
            }
        }
        
        public IEnumerable<Vector3Int> GetPositions()
        {
            for (int k = 0; k <= chunkMap.LayerLength; k++)
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
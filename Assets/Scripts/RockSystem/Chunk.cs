using Managers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RockSystem
{
    public class Chunk
    {
        private readonly ChunkDescription chunkDescription;
        private ChunkStructure chunkStructure;

        private readonly Vector3Int position;
        public Vector3Int Position => position;
        public Vector2Int FlatPosition => (Vector2Int) position;
        
        private int currentHealth;
        public int Health => currentHealth;
        public int MaxHealth => chunkDescription.Health;

        internal Chunk(ChunkDescription chunkDescription, Vector3Int position)
        {
            this.chunkDescription = chunkDescription;
            this.position = position;
        }

        internal void AttachTo(ChunkStructure chunkStructure)
        {
            this.chunkStructure = chunkStructure;
        }
        
        internal TileBase CreateTile()
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = chunkDescription.Sprite;
            tile.transform = Matrix4x4.Translate(new Vector3(chunkDescription.Offset.x, chunkDescription.Offset.y, 0));
            return tile;
        }

        internal void DamageChunk(int amount)
        {
            if (amount <= 0) return;
            
            Debug.Log($"Damaging by {amount} with chunk at layer {position.z}");
            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                chunkStructure.Clear(position);
            }
        }
    }
}
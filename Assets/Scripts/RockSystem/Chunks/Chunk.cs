using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace RockSystem.Chunks
{
    public class Chunk
    {
        private readonly ChunkDescription chunkDescription;

        private readonly Vector3Int position;
        private readonly Action<Chunk> chunkDestroyedBehaviour;
        public Vector3Int Position => position;
        public Vector2Int FlatPosition => (Vector2Int) position;
        
        private float currentHealth;
        public float Health => currentHealth;
        public float MaxHealth => chunkDescription.Health;

        internal Chunk(ChunkDescription chunkDescription, Vector3Int position, Action<Chunk> chunkDestroyedBehaviour)
        {
            this.chunkDescription = chunkDescription;
            this.position = position;
            this.chunkDestroyedBehaviour = chunkDestroyedBehaviour;

            float healthVariation = Random.Range(-chunkDescription.HealthVariation, chunkDescription.HealthVariation + 1);
            currentHealth = chunkDescription.Health + healthVariation;

            if (currentHealth < 0)
                currentHealth = 0;
        }

        internal TileBase CreateTile()
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = chunkDescription.Sprite;
            tile.transform = Matrix4x4.Translate(new Vector3(chunkDescription.Offset.x, chunkDescription.Offset.y, 0));
            return tile;
        }

        /// <summary>
        /// Deals the given amount of damage to the chunk and returns the damage taken.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>The amount of damage taken.</returns>
        internal float DamageChunk(float amount)
        {
            if (amount <= 0) return 0;

            float damageTaken = Mathf.Min(amount, currentHealth);
            
            // Debug.Log($"Damaging by {amount} with chunk at layer {position.z}");
            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                DestroyChunk();
            }

            return damageTaken;
        }

        private void DestroyChunk()
        {
            chunkDestroyedBehaviour.Invoke(this);
        }
    }
}
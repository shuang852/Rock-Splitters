using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace RockSystem
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
        private ChunkStructure chunkStructure;
        private Grid grid;

        protected override void Awake()
        {
            base.Awake();
            chunkMap = GetComponent<ChunkMap>();
            grid = GetComponent<Grid>();

            if (rocks.Count < 1)
                throw new InvalidOperationException($"No rocks assigned to the {nameof(ChunkManager)}!");

            chunkStructure = new ChunkStructure(size, chunkMap);
        }

        protected override void Start()
        {
            base.Start();
            
            GenerateChunks();
        }
        
        public void DamageChunk(Vector2 worldPosition)
        {
            Vector2Int flatPosition = (Vector2Int) grid.WorldToCell(worldPosition);
            DamageChunk(flatPosition);
        }

        public void DamageChunk(Vector2Int flatPosition)
        {
            chunkStructure.GetOrNull(flatPosition)?.DamageChunk(1);
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
    }
}
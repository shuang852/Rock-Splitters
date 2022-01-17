using System;
using System.Collections.Generic;
using Managers;
using RockSystem.Fossils;
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
        
        public void DamageChunk(Vector2 worldPosition)
        {
            Vector2Int flatPosition = chunkStructure.WorldToCell(worldPosition);
            DamageChunk(flatPosition);
        }

        public void DamageChunk(Vector2Int flatPosition)
        {
            Chunk chunk = chunkStructure.GetOrNull(flatPosition);

            if (chunk != null)
            {
                FossilShape fossil = GetFossilAtPosition(chunk.Position);

                if (fossil == null)
                {
                    chunk.DamageChunk(1);
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
    }
}
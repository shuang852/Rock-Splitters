using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;

namespace RockSystem.Fossils
{
    public class FossilShape : MonoBehaviour
    {
        [SerializeField] private int layer;
        [SerializeField] private string sortingLayer = "Chunk";
        [SerializeField] private bool enableDebug;

        private readonly HashSet<Vector2Int> hitFlatPositions = new HashSet<Vector2Int>();
        private SpriteRenderer spriteRenderer;
        private PolygonCollider2D polyCollider;

        private IEnumerable<Vector3Int> HitPositions =>
            hitFlatPositions.Select(p => new Vector3Int(p.x, p.y, layer));

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            polyCollider = GetComponent<PolygonCollider2D>();

            // Note that this will override sprite renderer settings
            spriteRenderer.sortingLayerName = sortingLayer;
            spriteRenderer.sortingOrder = layer;
        }

        private void Start()
        {
            CalculateHitPositions();
            ChunkManager chunkManager = M.GetOrThrow<ChunkManager>();
            chunkManager.RegisterFossil(this);
        }

        public bool IsHitAtPosition(Vector3Int position) => HitPositions.Contains(position);

        public bool IsHitAtFlatPosition(Vector2Int position) => hitFlatPositions.Contains(position);

        private void CalculateHitPositions()
        {
            ChunkManager chunkManager = M.GetOrThrow<ChunkManager>();

            foreach (Vector2Int flatPosition in chunkManager.chunkStructure.GetFlatPositions())
            {
                Vector3 worldPosition = chunkManager.chunkStructure.CellToWorld(flatPosition);

                if (polyCollider.OverlapPoint(worldPosition))
                {
                    hitFlatPositions.Add(flatPosition);
                }
            }
        }

        private void OnDrawGizmos()
        {
            ChunkManager chunkManager = M.GetOrNull<ChunkManager>();
            
            if (enableDebug && chunkManager != null)
            {
                Gizmos.color = Color.gray;
                
                foreach (Vector2Int position in hitFlatPositions)
                {
                    Vector3 worldPosition = chunkManager.chunkStructure.CellToWorld(position);
                    Gizmos.DrawSphere(worldPosition, 0.08f);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using RockSystem.Chunks;
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
        private Sprite Sprite => spriteRenderer.sprite;

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
                bool isHitPosition = false;
                float radius = chunkManager.chunkStructure.CellSize.x / 2f;
                Vector3 centerWorldPosition = chunkManager.chunkStructure.CellToWorld(flatPosition);
                var cornerPositions = GetHexagonCornerPositions(centerWorldPosition, radius)
                    .Append(flatPosition);

                foreach (var cornerPosition in cornerPositions)
                {
                    if (polyCollider.OverlapPoint(cornerPosition) && !isHitPosition)
                    {
                        Debug.Log($"hit corner {cornerPosition}");
                        hitFlatPositions.Add(flatPosition);
                        isHitPosition = true;
                    }
                }
            }
        }
        
        // TODO move this to utility class
        private IEnumerable<Vector2> GetHexagonCornerPositions(Vector2 centerWorldPosition, float radius) => new[]
        {
            new Vector2(radius, 0),
            new Vector2(-radius, 0),
            new Vector2(radius * Mathf.Cos(Mathf.Deg2Rad * 60f), radius * Mathf.Sin(Mathf.Deg2Rad * 60f)),
            new Vector2(radius * Mathf.Cos(Mathf.Deg2Rad * 120f), radius * Mathf.Sin(Mathf.Deg2Rad * 120f)),
            new Vector2(radius * Mathf.Cos(Mathf.Deg2Rad * 240f), radius * Mathf.Sin(Mathf.Deg2Rad * 240f)),
            new Vector2(radius * Mathf.Cos(Mathf.Deg2Rad * 300f), radius * Mathf.Sin(Mathf.Deg2Rad * 300f)),
        }.Select(p => p + centerWorldPosition);

        // private List<Vector2Int> CalculatePixelHitsWorldPositions(Sprite sprite)
        // {
        //     List<Vector2Int> pixel
        //
        //     foreach (var pixel in sprite.texture.GetPixels32())
        //     {
        //         if (pixel.r)
        //     }
        // }

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
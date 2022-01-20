using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using RockSystem.Chunks;
using Stored;
using UnityEngine;

namespace RockSystem.Fossils
{
    public class FossilShape : MonoBehaviour
    {
        [SerializeField] private Antiquity fossil;
        [SerializeField] private int layer;
        [SerializeField] private string sortingLayer = "Chunk";
        [SerializeField] private bool enableDebug;

        private readonly Dictionary<Vector2Int, int> chunkHealths = new Dictionary<Vector2Int, int>();
        private SpriteRenderer spriteRenderer;
        private SpriteMask spriteMask;
        private PolygonCollider2D polyCollider;

        private IEnumerable<Vector3Int> HitPositions =>
            chunkHealths.Keys.Select(p => new Vector3Int(p.x, p.y, layer));
        private IEnumerable<Vector2Int> HitFlatPositions => chunkHealths.Keys;
        private Sprite Sprite => Antiquity.Sprite;
        public Antiquity Antiquity => fossil;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteMask = GetComponent<SpriteMask>();

            // Note that this will override sprite renderer settings
            spriteRenderer.sortingLayerName = sortingLayer;
            spriteRenderer.sortingOrder = layer;
            
            // Setup sprites according to antiquity
            spriteRenderer.sprite = Sprite;
            spriteMask.sprite = Sprite;
            
            // Setup colliders
            polyCollider = gameObject.AddComponent<PolygonCollider2D>();
        }

        private void Start()
        {
            SetupFossilChunks();
            ChunkManager chunkManager = M.GetOrThrow<ChunkManager>();
            chunkManager.RegisterFossil(this);
        }

        private void SetupFossilChunks()
        {
            ChunkManager chunkManager = M.GetOrThrow<ChunkManager>();
            int startingHealth = fossil.MaxHealth;

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
                        chunkHealths.Add(flatPosition, startingHealth);
                        isHitPosition = true;
                    }
                }
            }
        }

        #region Damage

        public void DamageFossilChunk(Vector2Int position, int amount)
        {
            if (!chunkHealths.ContainsKey(position))
                throw new IndexOutOfRangeException($"position {position} is not a valid fossil chunk");

            chunkHealths[position] = Mathf.Max(0, chunkHealths[position] - amount);
        }

        public int GetFossilChunkHealth(Vector2Int position) =>
            chunkHealths.ContainsKey(position) ? chunkHealths[position] : 0;

        #endregion
        
        public bool IsHitAtPosition(Vector3Int position) => HitPositions.Contains(position);

        public bool IsHitAtFlatPosition(Vector2Int position) => HitFlatPositions.Contains(position);
        
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
                
                foreach (Vector2Int position in HitFlatPositions)
                {
                    Vector3 worldPosition = chunkManager.chunkStructure.CellToWorld(position);
                    Gizmos.DrawSphere(worldPosition, 0.08f);
                }
            }
        }
    }
}
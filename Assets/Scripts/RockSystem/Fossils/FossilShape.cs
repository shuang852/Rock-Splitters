using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using RockSystem.Chunks;
using Stored;
using UnityEngine;
using UnityEngine.Events;

namespace RockSystem.Fossils
{
    // TODO: Temporarily a manager to be accessed by XRayManager. May need reworking.
    public class FossilShape : Manager
    {
        [SerializeField] private Antiquity fossil;
        [SerializeField] private int layer;
        [SerializeField] private string sortingLayer = "Chunk";
        [SerializeField] private bool enableDebug;

        private readonly Dictionary<Vector2Int, int> chunkHealths = new Dictionary<Vector2Int, int>();
        private SpriteRenderer spriteRenderer;
        private SpriteMask spriteMask;
        private PolygonCollider2D polyCollider;
        private ChunkManager chunkManager;

        private IEnumerable<Vector2Int> HitFlatPositions => chunkHealths.Keys;
        private Sprite Sprite => Antiquity.Sprite;
        public Antiquity Antiquity => fossil;

        public UnityEvent fossilDamaged = new UnityEvent();

        protected override void Awake()
        {
            base.Awake();
            
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

        protected override void Start()
        {
            base.Start();

            chunkManager = M.GetOrThrow<ChunkManager>();
            SetupFossilChunks();
            chunkManager.RegisterFossil(this);
        }

        private void SetupFossilChunks()
        {
            int startingHealth = fossil.MaxHealth;
            
            float radius = chunkManager.chunkStructure.CellSize.x / 2f;

            foreach (Vector2Int flatPosition in chunkManager.chunkStructure.FlatPositions)
            {
                Vector3 centerWorldPosition = chunkManager.chunkStructure.CellToWorld(flatPosition);
                var cornerPositions = GetHexagonCornerPositions(centerWorldPosition, radius)
                    .Append(flatPosition);

                if (cornerPositions.Any(cornerPosition => polyCollider.OverlapPoint(cornerPosition)))
                    chunkHealths.Add(flatPosition, startingHealth);
            }
        }

        #region Damage

        public void DamageFossilChunk(Vector2Int position, int amount)
        {
            if (!chunkHealths.ContainsKey(position))
                throw new IndexOutOfRangeException($"position {position} is not a valid fossil chunk");

            chunkHealths[position] = Mathf.Max(0, chunkHealths[position] - amount);
            
            fossilDamaged.Invoke();
        }

        public int GetFossilChunkHealth(Vector2Int position) =>
            chunkHealths.ContainsKey(position) ? chunkHealths[position] : 0;

        #endregion

        public bool IsHitAtPosition(Vector3Int position) =>
            position.z == layer && chunkHealths.ContainsKey(new Vector2Int(position.x, position.y));

        public bool IsHitAtFlatPosition(Vector2Int position) => chunkHealths.ContainsKey(position);
        
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
        
        public float FossilHealth ()
        {
            float currentTotalHealth = chunkHealths.Values.Sum();

            float maxTotalHealth = chunkHealths.Count * fossil.MaxHealth;
            
            return currentTotalHealth / maxTotalHealth;
        }

        public float FossilExposure()
        {
            int exposedChunks = chunkHealths.Keys.Count(i => chunkManager.GetFossilAtPosition(i) == this);
            
            int totalChunks = chunkHealths.Count;

            return exposedChunks / (float) totalChunks;
        }
    }
}
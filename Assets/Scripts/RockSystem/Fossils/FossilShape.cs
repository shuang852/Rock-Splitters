using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using RockSystem.Chunks;
using Stored;
using UnityEngine;
using UnityEngine.Events;
using Utility;

namespace RockSystem.Fossils
{
    public class FossilShape : Manager
    {
        [SerializeField] private Antiquity fossil;
        [SerializeField] private int layer;
        [SerializeField] private string sortingLayer = "Chunk";
        [SerializeField] private bool enableDebug;

        private readonly Dictionary<Vector2Int, float> chunkHealths = new Dictionary<Vector2Int, float>();
        private readonly Dictionary<Vector2Int, bool> chunkExposure = new Dictionary<Vector2Int, bool>();
        private SpriteRenderer spriteRenderer;
        private SpriteMask spriteMask;
        private PolygonCollider2D polyCollider;
        private ChunkManager chunkManager;

        private IEnumerable<Vector2Int> HitFlatPositions => chunkHealths.Keys;
        private Sprite Sprite => Antiquity.Sprite;
        public Antiquity Antiquity => fossil;

        public UnityEvent fossilExposed = new UnityEvent();
        public UnityEvent fossilDamaged = new UnityEvent();

        private bool exposureChanged;
        private bool healthChanged;
        private float fossilExposure;
        private float fossilHealth;

        public float FossilExposure
        {
            get => fossilExposure;
            private set
            {
                fossilExposure = value;
                fossilExposed.Invoke();
            }
        }

        public float FossilHealth
        {
            get => fossilHealth;
            private set
            {
                fossilHealth = value;
                fossilDamaged.Invoke();
            }
        }

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
            chunkManager.chunkCleared.AddListener(OnChunkDestroyed);
        }
        
        private void OnChunkDestroyed(Chunk chunk)
        {
            Vector3Int underChunk = chunk.Position + Vector3Int.back;

            if (!IsHitAtPosition(underChunk)) return;
            
            exposureChanged = true;

            chunkExposure[chunk.FlatPosition] = true;
        }

        private void SetupFossilChunks()
        {
            float startingHealth = fossil.MaxHealth;
            
            float radius = chunkManager.chunkStructure.CellSize.x / 2f;

            var overlappingPositions =
                GetOverlappingFlatPositions(chunkManager.chunkStructure.FlatPositions, radius, polyCollider);
            
            foreach (Vector2Int flatPosition in overlappingPositions)
            {
                chunkHealths.Add(flatPosition, startingHealth);
            }
        }

        private IEnumerable<Vector2Int> GetOverlappingFlatPositions(IEnumerable<Vector2Int> flatPositions, float radius, Collider2D collider)
        {
            List<Vector2Int> output = new List<Vector2Int>();
            
            foreach (Vector2Int flatPosition in chunkManager.chunkStructure.FlatPositions)
            {
                Vector3 centerWorldPosition = chunkManager.chunkStructure.CellToWorld(flatPosition);
                var cornerPositions = Hexagons.GetHexagonCornerPositions(centerWorldPosition, radius)
                    .Append(flatPosition);

                if (cornerPositions.Any(cornerPosition => polyCollider.OverlapPoint(cornerPosition)))
                    output.Add(flatPosition);
            }

            return output;
        }

        #region Damage

        public void DamageFossilChunk(Vector2Int position, float amount)
        {
            if (!chunkHealths.ContainsKey(position))
                throw new IndexOutOfRangeException($"position {position} is not a valid fossil chunk");

            chunkHealths[position] = Mathf.Max(0, chunkHealths[position] - amount);

            healthChanged = true;
        }

        public float GetFossilChunkHealth(Vector2Int position) =>
            chunkHealths.ContainsKey(position) ? chunkHealths[position] : 0;

        #endregion

        public bool IsHitAtPosition(Vector3Int position) =>
            position.z == layer && chunkHealths.ContainsKey(new Vector2Int(position.x, position.y));

        public bool IsHitAtFlatPosition(Vector2Int position) => chunkHealths.ContainsKey(position);
        
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

        private void UpdateFossilHealth ()
        {
            float currentTotalHealth = chunkHealths.Values.Sum();

            float maxTotalHealth = chunkHealths.Count * fossil.MaxHealth;
            
            FossilHealth = currentTotalHealth / maxTotalHealth;
        }

        private void UpdateFossilExposure()
        {
            int exposedChunks = chunkExposure.Count(i => i.Value);
            
            int totalChunks = chunkHealths.Count;

            FossilExposure = exposedChunks / (float) totalChunks;
        }

        public void CheckHealthAndExposure()
        {
            if (exposureChanged)
            {
                UpdateFossilExposure();
                exposureChanged = false;
            }

            if (healthChanged)
            {
                UpdateFossilHealth();
                healthChanged = false;
            }
        }
    }
}
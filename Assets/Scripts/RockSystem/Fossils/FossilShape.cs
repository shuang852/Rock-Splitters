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
        [SerializeField] private bool enableDebug;
        
        // TODO: Changing the fossil layer is no longer supported.
        private readonly int layer = 0;

        private readonly Dictionary<Vector2Int, float> chunkHealths = new Dictionary<Vector2Int, float>();
        private readonly Dictionary<Vector2Int, bool> chunkExposure = new Dictionary<Vector2Int, bool>();
        private SpriteRenderer spriteRenderer;
        private SpriteMask spriteMask;
        private PolygonCollider2D polyCollider;
        private ChunkManager chunkManager;
        private ArtefactManager artefactManager;

        private IEnumerable<Vector2Int> HitFlatPositions => chunkHealths.Keys;
        private Sprite Sprite => Antiquity.Sprite;
        public Antiquity Antiquity => fossil;

        public UnityEvent initialised = new UnityEvent();
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
        }

        protected override void Start()
        {
            base.Start();

            chunkManager = M.GetOrThrow<ChunkManager>();
            artefactManager = M.GetOrThrow<ArtefactManager>();
            
            chunkManager.chunkCleared.AddListener(OnChunkDestroyed);
        }

        public void Initialise(Antiquity antiquity)
        {
            fossil = antiquity; 
            
            // Setup sprites according to antiquity

            spriteRenderer.sprite = Sprite;
            spriteMask.sprite = Sprite;
            
            // Setup colliders
            polyCollider = gameObject.AddComponent<PolygonCollider2D>();
            
            SetupFossilChunks();
            artefactManager.RegisterFossil(this);
            
            ForceUpdateFossilExposure();
            
            initialised.Invoke();
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
            
            foreach (Vector2Int flatPosition in chunkManager.chunkStructure.FlatPositions)
            {
                if (Hexagons.HexagonOverlapsCollider(chunkManager.CurrentGrid, flatPosition, polyCollider))
                    chunkHealths.Add(flatPosition, startingHealth);
            }

            FossilHealth = 1;
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

        public bool IsExposedAtFlatPosition(Vector2Int flatPosition)
        {
            if (!IsHitAtFlatPosition(flatPosition)) return false;

            return chunkManager.chunkStructure.GetOrNull(flatPosition) == null;
        }

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
        
        // TODO: Needs a better name.
        public void ForceUpdateFossilExposure()
        {
            int exposedChunks = 0;
            
            foreach (var flatPosition in chunkHealths.Keys)
            {
                if (IsExposedAtFlatPosition(flatPosition))
                {
                    chunkExposure[flatPosition] = true;

                    exposedChunks++;
                }
                else
                {
                    chunkExposure[flatPosition] = false;
                }
            }

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
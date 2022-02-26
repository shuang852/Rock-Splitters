﻿using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;
using UnityEngine.Events;
using Utility;

namespace RockSystem.Chunks
{
    [RequireComponent(typeof(SpriteRenderer), typeof(SpriteMask))]
    public abstract class ChunkShape : MonoBehaviour
    {
        [SerializeField] protected Sprite sprite;
        [SerializeField] protected float maxHealth;
        [SerializeField] private bool enableDebug;
        
        public UnityEvent initialised = new UnityEvent();
        public UnityEvent exposed = new UnityEvent();
        public UnityEvent damaged = new UnityEvent();
        public UnityEvent<ChunkShape, Vector2Int> chunkDamaged = new UnityEvent<ChunkShape, Vector2Int>();
        
        public float Exposure
        {
            get => exposure;
            private set
            {
                exposure = value;
                exposed.Invoke();
            }
        }

        public float Health
        {
            get => health;
            private set
            {
                health = value;
                damaged.Invoke();
            }
        }

        public int ExposedChunks { get; private set; }

        public int NumOfChunks => chunkHealths.Count;

        public readonly Dictionary<Vector2Int, bool> ChunkExposure = new Dictionary<Vector2Int, bool>();
        public bool CanBeDamaged { get; set; } = true;
        public Sprite Sprite => sprite;
        public int Layer => layer;
        
        private readonly Dictionary<Vector2Int, float> chunkHealths = new Dictionary<Vector2Int, float>();
        private SpriteRenderer spriteRenderer;
        private SpriteMask spriteMask;
        private PolygonCollider2D polyCollider;
        private ChunkManager chunkManager;

        private IEnumerable<Vector2Int> HitFlatPositions => chunkHealths.Keys;
        // TODO: Inefficient, should be cached.
        private IEnumerable<Vector3Int> HitPositions => chunkHealths.Keys.Select(v => new Vector3Int(v.x, v.y, layer));

        private bool exposureChanged;
        private bool healthChanged;
        private float exposure;
        private float health;
        
        private int layer;
        private string sortingLayer = "Chunk";

        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteMask = GetComponent<SpriteMask>();
        }

        protected virtual void Start()
        {
            GetManagers();
        }

        private void GetManagers()
        {
            chunkManager = M.GetOrThrow<ChunkManager>();

            chunkManager.chunkCleared.AddListener(OnChunkDestroyed);
        }

        // TODO: Setup sorting layer and order in layer
        protected void Initialise(Sprite sprite, float maxHealth, int layer)
        {
            GetManagers();
            
            this.sprite = sprite;
            this.maxHealth = maxHealth;
            this.layer = layer;
            
            // Setup sprites
            spriteRenderer.sprite = sprite;
            spriteMask.sprite = sprite;

            spriteRenderer.sortingLayerName = sortingLayer;
            spriteRenderer.sortingOrder = layer - 1;
            
            // Setup colliders
            if (polyCollider != null)
                Destroy(polyCollider);
            
            polyCollider = gameObject.AddComponent<PolygonCollider2D>();
            
            SetupChunks();
            
            ForceUpdateExposure();
            
            initialised.Invoke();
        }
        
        private void OnChunkDestroyed(Chunk chunk)
        {
            if (!IsExposedAtFlatPosition(chunk.FlatPosition)) return;

            exposureChanged = true;

            ChunkExposure[chunk.FlatPosition] = true;
        }

        private void SetupChunks()
        {
            chunkHealths.Clear();
            ChunkExposure.Clear();
            
            foreach (Vector2Int flatPosition in chunkManager.ChunkStructure.FlatPositions)
            {
                if (Hexagons.HexagonOverlapsCollider(chunkManager.CurrentGrid, flatPosition, polyCollider))
                    chunkHealths.Add(flatPosition, maxHealth);
            }

            Health = 1;
        }

        #region Damage

        public void DamageChunk(Vector2Int position, float amount)
        {
            if (!chunkHealths.ContainsKey(position))
                throw new IndexOutOfRangeException($"Position {position} is not in the {nameof(ChunkShape)}");

            if (!CanBeDamaged) return;

            chunkHealths[position] = Mathf.Max(0, chunkHealths[position] - amount);

            healthChanged = true;
            
            chunkDamaged.Invoke(this, position);
        }

        public float GetChunkHealth(Vector2Int position) =>
            chunkHealths.ContainsKey(position) ? chunkHealths[position] : 0;

        #endregion

        public bool IsHitAtFlatPosition(Vector2Int position) => chunkHealths.ContainsKey(position);

        public bool IsExposedAtFlatPosition(Vector2Int flatPosition)
        {
            if (!IsHitAtFlatPosition(flatPosition)) return false;

            return chunkManager.ChunkStructure.GetOrNull(flatPosition) == null;
        }
        
        public bool IsHitAtPosition(Vector3Int position)
        {
            return HitPositions.Contains(position);
        }

        protected virtual void OnDrawGizmos()
        {
            ChunkManager chunkManager = M.GetOrNull<ChunkManager>();
            
            if (enableDebug && chunkManager != null)
            {
                Gizmos.color = Color.gray;
                
                foreach (Vector2Int position in HitFlatPositions)
                {
                    Vector3 worldPosition = chunkManager.ChunkStructure.CellToWorld(position);
                    Gizmos.DrawSphere(worldPosition, 0.08f);
                }
            }
        }

        // TODO: Simplify
        private void UpdateHealth ()
        {
            float currentTotalHealth = CurrentTotalHealth;

            float maxTotalHealth = MaxTotalHealth;
            
            Health = currentTotalHealth / maxTotalHealth;
        }

        // TODO: Move to top
        public float MaxTotalHealth => chunkHealths.Count * maxHealth;

        public float CurrentTotalHealth => chunkHealths.Values.Sum();

        private void UpdateExposure()
        {
            ExposedChunks = ChunkExposure.Count(i => i.Value);
            
            int totalChunks = chunkHealths.Count;

            Exposure = ExposedChunks / (float) totalChunks;
        }
        
        // TODO: Needs a better name.
        public void ForceUpdateExposure()
        {
            int exposedChunks = 0;
            
            foreach (var flatPosition in chunkHealths.Keys)
            {
                if (IsExposedAtFlatPosition(flatPosition))
                {
                    ChunkExposure[flatPosition] = true;

                    exposedChunks++;
                }
                else
                {
                    ChunkExposure[flatPosition] = false;
                }
            }

            ExposedChunks = exposedChunks;

            int totalChunks = chunkHealths.Count;

            Exposure = ExposedChunks / (float) totalChunks;
        }

        public void CheckHealthAndExposure()
        {
            CheckExposure();

            CheckHealth();
        }

        // TODO: Summary
        public bool CheckHealth()
        {
            if (!healthChanged) return false;
            
            UpdateHealth();
            healthChanged = false;

            return true;
        }

        // TODO: Summary
        public bool CheckExposure()
        {
            if (!exposureChanged) return false;
            
            UpdateExposure();
            exposureChanged = false;

            return true;
        }

        protected virtual void OnDestroy()
        {
            chunkManager.chunkCleared.RemoveListener(OnChunkDestroyed);
        }
    }
}
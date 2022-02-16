using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using RockSystem.Chunks;
using Stored;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Utility;

namespace RockSystem.Artefacts
{
    public class ArtefactShape : Manager
    {
        [FormerlySerializedAs("fossil")] [SerializeField] private Artefact artefact;
        [SerializeField] private bool enableDebug;

        private readonly Dictionary<Vector2Int, float> chunkHealths = new Dictionary<Vector2Int, float>();
        public readonly Dictionary<Vector2Int, bool> ChunkExposure = new Dictionary<Vector2Int, bool>();
        private SpriteRenderer spriteRenderer;
        private SpriteMask spriteMask;
        private PolygonCollider2D polyCollider;
        private ChunkManager chunkManager;
        private ArtefactShapeManager artefactShapeManager;

        private IEnumerable<Vector2Int> HitFlatPositions => chunkHealths.Keys;
        public Artefact Artefact => artefact;

        public UnityEvent initialised = new UnityEvent();
        public UnityEvent artefactExposed = new UnityEvent();
        public UnityEvent artefactDamaged = new UnityEvent();

        private bool exposureChanged;
        private bool healthChanged;
        private float artefactExposure;
        private float artefactHealth;

        public float ArtefactExposure
        {
            get => artefactExposure;
            private set
            {
                artefactExposure = value;
                artefactExposed.Invoke();
            }
        }

        public float ArtefactHealth
        {
            get => artefactHealth;
            private set
            {
                artefactHealth = value;
                artefactDamaged.Invoke();
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
            artefactShapeManager = M.GetOrThrow<ArtefactShapeManager>();
            
            chunkManager.chunkCleared.AddListener(OnChunkDestroyed);
        }

        public void Initialise(Artefact artefact)
        {
            this.artefact = artefact; 
            
            // Setup sprites according to the artefact
            spriteRenderer.sprite = artefact.Sprite;
            spriteMask.sprite = artefact.Sprite;
            
            // Setup colliders
            if (polyCollider != null)
                Destroy(polyCollider);
            
            polyCollider = gameObject.AddComponent<PolygonCollider2D>();
            
            SetupArtefactChunks();
            
            artefactShapeManager.UnregisterArtefact(this);
            artefactShapeManager.RegisterArtefact(this);
            
            ForceUpdateArtefactExposure();
            
            initialised.Invoke();
        }
        
        private void OnChunkDestroyed(Chunk chunk)
        {
            if (!IsExposedAtFlatPosition(chunk.FlatPosition)) return;

            exposureChanged = true;

            ChunkExposure[chunk.FlatPosition] = true;
        }

        private void SetupArtefactChunks()
        {
            chunkHealths.Clear();
            ChunkExposure.Clear();
            
            foreach (Vector2Int flatPosition in chunkManager.ChunkStructure.FlatPositions)
            {
                if (Hexagons.HexagonOverlapsCollider(chunkManager.CurrentGrid, flatPosition, polyCollider))
                    chunkHealths.Add(flatPosition, artefact.MaxHealth);
            }

            ArtefactHealth = 1;
        }

        #region Damage

        public void DamageArtefactChunk(Vector2Int position, float amount)
        {
            if (!chunkHealths.ContainsKey(position))
                throw new IndexOutOfRangeException($"position {position} is not a valid artefact chunk");

            chunkHealths[position] = Mathf.Max(0, chunkHealths[position] - amount);

            healthChanged = true;
        }

        public float GetArtefactChunkHealth(Vector2Int position) =>
            chunkHealths.ContainsKey(position) ? chunkHealths[position] : 0;

        #endregion

        public bool IsHitAtFlatPosition(Vector2Int position) => chunkHealths.ContainsKey(position);

        public bool IsExposedAtFlatPosition(Vector2Int flatPosition)
        {
            if (!IsHitAtFlatPosition(flatPosition)) return false;

            return chunkManager.ChunkStructure.GetOrNull(flatPosition) == null;
        }

        private void OnDrawGizmos()
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

        private void UpdateArtefactHealth ()
        {
            float currentTotalHealth = chunkHealths.Values.Sum();

            float maxTotalHealth = chunkHealths.Count * artefact.MaxHealth;
            
            ArtefactHealth = currentTotalHealth / maxTotalHealth;
        }

        private void UpdateArtefactExposure()
        {
            int exposedChunks = ChunkExposure.Count(i => i.Value);
            
            int totalChunks = chunkHealths.Count;

            ArtefactExposure = exposedChunks / (float) totalChunks;
        }
        
        // TODO: Needs a better name.
        public void ForceUpdateArtefactExposure()
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

            int totalChunks = chunkHealths.Count;

            ArtefactExposure = exposedChunks / (float) totalChunks;
        }

        public void CheckHealthAndExposure()
        {
            if (exposureChanged)
            {
                UpdateArtefactExposure();
                exposureChanged = false;
            }

            if (healthChanged)
            {
                UpdateArtefactHealth();
                healthChanged = false;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            artefactShapeManager.UnregisterArtefact(this);
            
            chunkManager.chunkCleared.RemoveListener(OnChunkDestroyed);
        }
    }
}
using System;
using System.Collections.Generic;
using Managers;
using RockSystem.Chunks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RockSystem.Artefacts
{
    public class DamageLayer : MonoBehaviour
    {
        [SerializeField] private string sortingLayer;
        [SerializeField] private int sortingOrder;
        [SerializeField] private List<Sprite> damageSprites;
        [SerializeField] private Sprite bustedDamageSprite;

        private readonly List<Tile> damageTiles = new List<Tile>();
        private Tile bustedDamageTile;
        private Tilemap tilemap;

        private ArtefactShapeManager artefactShapeManager;

        protected void Awake()
        {
            CreateTilemap();
            
            CreateDamageTiles();
        }

        protected void Start()
        {
            artefactShapeManager = M.GetOrThrow<ArtefactShapeManager>();
            
            artefactShapeManager.artefactChunkDamaged.AddListener(OnArtefactChunkDamaged);
            artefactShapeManager.initialised.AddListener(Initialise);
        }

        private void Initialise()
        {
            tilemap.ClearAllTiles();
        }

        private void OnArtefactChunkDamaged(ArtefactShape artefact, Vector2Int flatPosition)
        {
            float remainingHealth = artefact.GetChunkHealth(flatPosition);
        
            if (artefact.Artefact.MaxHealth <= 0)
            {
                Debug.LogError($"{nameof(artefact.Artefact.MaxHealth)} has not been set.");
                return;
            }
        
            if (!(remainingHealth <= artefact.Artefact.BreakingHealth)) return;
            
            float damagePercentage = 1f - (remainingHealth / artefact.Artefact.MaxHealth);
            DisplayDamage(flatPosition, damagePercentage);
        }

        private void CreateTilemap()
        {
            GameObject go = new GameObject($"Damage Layer Tilemap");
            go.transform.parent = transform;
            tilemap = go.AddComponent<Tilemap>();
            TilemapRenderer tilemapRenderer = go.AddComponent<TilemapRenderer>();

            tilemapRenderer.sortingLayerName = sortingLayer;
            tilemapRenderer.sortingOrder = sortingOrder;
            tilemapRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            
            // Fix tile offset from grid
            tilemap.tileAnchor = Vector3.zero;
        }

        private void CreateDamageTiles()
        {
            if (damageSprites.Count == 0)
                throw new Exception("Missing damage sprites, they are not assigned!");
            
            if (bustedDamageSprite == null)
                throw new Exception("Missing busted damage sprite, they are not assigned!");
            
            damageTiles.Clear();

            foreach (var sprite in damageSprites)
            {
                Tile tile = ScriptableObject.CreateInstance<Tile>();
                tile.sprite = sprite;
                damageTiles.Add(tile);
            }
            
            bustedDamageTile = ScriptableObject.CreateInstance<Tile>();
            bustedDamageTile.sprite = bustedDamageSprite;
        }

        public void DisplayDamage(Vector2Int flatPosition, float percentage)
        {
            TileBase damageTile = GetDamageTile(percentage);
            tilemap.SetTile((Vector3Int) flatPosition, damageTile);
        }

        private TileBase GetDamageTile(float percentage)
        {
            if (percentage < 0 || percentage > 1)
                throw new ArgumentException($"{percentage} is not a valid percentage!");

            return percentage >= 1
                ? bustedDamageTile
                : damageTiles[Mathf.RoundToInt(percentage * (damageTiles.Count - 1))];
        }

        private void OnDestroy()
        {
            artefactShapeManager.artefactChunkDamaged.RemoveListener(OnArtefactChunkDamaged);
            artefactShapeManager.initialised.RemoveListener(Initialise);
        }
    }
}
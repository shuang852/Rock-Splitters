using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RockSystem.Fossils
{
    public class DamageLayer : MonoBehaviour
    {
        [SerializeField] private string sortingLayer = "Chunk";
        [SerializeField] private int sortingOrder = 20;
        [SerializeField] private List<Sprite> damageSprites;
        [SerializeField] private Sprite bustedDamageSprite;

        private readonly List<Tile> damageTiles = new List<Tile>();
        private Tile bustedDamageTile;
        private Tilemap tilemap;

        private CleaningArtefactManager cleaningArtefactManager;

        protected void Awake()
        {
            CreateTilemap();
            
            CreateDamageTiles();
        }

        protected void Start()
        {
            cleaningArtefactManager = M.GetOrThrow<CleaningArtefactManager>();
            
            cleaningArtefactManager.fossilDamaged.AddListener(OnFossilDamaged);
        }

        private void OnFossilDamaged(FossilShape fossil, Vector2Int flatPosition)
        {
            float remainingHealth = fossil.GetFossilChunkHealth(flatPosition);

            if (fossil.Artefact.MaxHealth <= 0)
            {
                Debug.LogError($"{nameof(fossil.Artefact.MaxHealth)} has not been set.");
                return;
            }

            if (!(remainingHealth <= fossil.Artefact.BreakingHealth)) return;
            
            float damagePercentage = 1f - (remainingHealth / fossil.Artefact.MaxHealth);
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
            
            // Debug.Log($"Displaying damage level {level} at {flatPosition}");
        }

        private TileBase GetDamageTile(float percentage)
        {
            if (percentage < 0 || percentage > 1)
                throw new ArgumentException($"{percentage} is not a valid percentage!");

            return percentage >= 1
                ? bustedDamageTile
                : damageTiles[Mathf.RoundToInt(percentage * (damageTiles.Count - 1))];
        }
    }
}
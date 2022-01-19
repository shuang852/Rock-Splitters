using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RockSystem.Fossils
{
    public class DamageLayer : MonoBehaviour
    {
        [SerializeField, Min(1)] private int damageLevelMin = 3;
        [SerializeField] private int damageLevelMax = 5;
        [SerializeField] private string sortingLayer = "Chunk";
        [SerializeField] private int sortingOrder = 20;
        [SerializeField] private List<Sprite> damageSprites;

        private Dictionary<Vector2Int, int> damageLevels = new Dictionary<Vector2Int, int>();
        private List<Tile> damageTiles = new List<Tile>();
        private Tilemap tilemap;

        private void Awake()
        {
            CreateTilemap();
            CreateDamageTiles();
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
            
            damageTiles.Clear();

            foreach (var sprite in damageSprites)
            {
                Tile tile = ScriptableObject.CreateInstance<Tile>();
                tile.sprite = sprite;
                damageTiles.Add(tile);
            }
        }

        public void DisplayDamage(Vector2Int flatPosition)
        {
            int level = AddAndGetDamageLevel(flatPosition);
            Tile damageTile = GetDamageTileOrNull(level);
            tilemap.SetTile((Vector3Int) flatPosition, damageTile);
            
            Debug.Log($"Displaying damage level {level} at {flatPosition}");
        }

        private Tile GetDamageTileOrNull(int level)
        {
            if (level <= 0)
                throw new IndexOutOfRangeException($"No level below 0 exists for a damage tile!");
            
            if (level > damageLevelMax)
                throw new IndexOutOfRangeException($"No level above {level} exists for a damage tile!");

            if (level <= damageLevelMin)
                return null;

            float range = damageLevelMax - damageLevelMin;
            float start = level - damageLevelMin;
            return damageTiles[Mathf.RoundToInt((start / range) * (damageTiles.Count - 1))];
        }

        private int GetDamageLevel(Vector2Int flatPosition)
        {
            return damageLevels.ContainsKey(flatPosition) ? damageLevels[flatPosition] : 0;
        }

        private int AddAndGetDamageLevel(Vector2Int flatPosition)
        {
            if (!damageLevels.ContainsKey(flatPosition))
            {
                damageLevels[flatPosition] = 0;
            }

            damageLevels[flatPosition] = Mathf.Min(damageLevelMax, damageLevels[flatPosition] + 1);
            return damageLevels[flatPosition];
        }
    }
}
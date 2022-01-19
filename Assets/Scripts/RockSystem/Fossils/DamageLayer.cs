using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RockSystem.Fossils
{
    public class DamageLayer : MonoBehaviour
    {
        [SerializeField] private int damageLevelSize = 5;
        [SerializeField] private Sprite damageSprite;
        [SerializeField] private float alphaGradient = 8f;
        [SerializeField] private string sortingLayer = "Chunk";
        [SerializeField] private int sortingOrder = 20;

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
            damageTiles.Clear();

            for (int i = 0; i < damageLevelSize; i++)
            {
                Tile tile = ScriptableObject.CreateInstance<Tile>();
                tile.sprite = damageSprite;
                float alphaValue = 1f / (i / alphaGradient + 1f);
                tile.color = new Color(1f, 1f, 1f, alphaValue);
                damageTiles.Insert(0, tile);
            }
        }

        public void DisplayDamage(Vector2Int flatPosition)
        {
            int level = AddAndGetDamageLevel(flatPosition);
            Tile damageTile = GetDamageTile(level);
            tilemap.SetTile((Vector3Int) flatPosition, damageTile);
            
            // Debug.Log($"Displaying damage at {flatPosition}");
        }

        private Tile GetDamageTile(int level) => level >= 0 || level < damageLevelSize
            ? damageTiles[level]
            : throw new IndexOutOfRangeException($"No level above {level} or below 0 exists for a tile!");

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

            damageLevels[flatPosition] = Mathf.Min(damageLevelSize - 1, damageLevels[flatPosition] + 1);
            return damageLevels[flatPosition];
        }
    }
}
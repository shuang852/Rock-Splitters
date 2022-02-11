using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RockSystem.Chunks
{
    /// <summary>
    /// Gives support for chunks to exist in a tilemap within a layering system.
    /// </summary>
    [RequireComponent(typeof(Grid))]
    internal class ChunkMap : MonoBehaviour
    {
        [SerializeField] private Color rockColor;
        [Tooltip("How different are colors between layers. Higher value means lesser difference")]
        [SerializeField, Min(0)] private float colorGradient = 8f;
        [SerializeField] private string tilemapSortingLayer = "Default";

        private readonly List<Tilemap> layeredTilemaps = new List<Tilemap>();

        public int LayerLength { get; set; }

        public void CreateTilemaps()
        {
            layeredTilemaps.Clear();
            
            for (int i = 0; i < LayerLength; i++)
            {
                GameObject go = new GameObject($"Layered Tilemap [layer {i}]");
                go.transform.parent = transform;
                Tilemap tilemap = go.AddComponent<Tilemap>();
                TilemapRenderer tilemapRenderer = go.AddComponent<TilemapRenderer>();
                
                // Fix tile offset from grid
                tilemap.tileAnchor = Vector3.zero;

                // Produce a darker color as we go deeper
                float colorValue = 1f / ((LayerLength - i) / colorGradient + 1f);
                tilemap.color = new Color(
                    rockColor.r * colorValue,
                    rockColor.g * colorValue,
                    rockColor.b * colorValue,
                    1f
                );
                
                tilemapRenderer.sortingOrder = i;
                tilemapRenderer.sortingLayerName = tilemapSortingLayer;

                layeredTilemaps.Add(tilemap);
            }
        }

        public void SetTileAtLayer(Vector3Int position, TileBase tile)
        {
            layeredTilemaps[position.z].SetTile(position, tile);
        }

        public void ClearTileAtLayer(Vector3Int position)
        {
            layeredTilemaps[position.z].SetTile(position, null);
        }

        public void HideRock()
        {
            foreach (var layeredTilemap in layeredTilemaps)
            {
                layeredTilemap.gameObject.SetActive(false);
            }
        }
    }
}
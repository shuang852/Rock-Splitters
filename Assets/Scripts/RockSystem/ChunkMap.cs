using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RockSystem
{
    /// <summary>
    /// Gives support for chunks to exist in a tilemap within a layering system.
    /// </summary>
    [RequireComponent(typeof(Grid))]
    internal class ChunkMap : MonoBehaviour
    {
        [SerializeField] private int layerLength = 1;
        [SerializeField] private string tilemapSortingLayer = "Default";

        private readonly List<Tilemap> layeredTilemaps = new List<Tilemap>();

        public int LayerLength => layerLength;

        private void Awake()
        {
            CreateTilemaps();
        }

        private void CreateTilemaps()
        {
            layeredTilemaps.Clear();
            
            for (int i = 0; i < layerLength; i++)
            {
                GameObject go = new GameObject($"Layered Tilemap [layer {i}]");
                go.transform.parent = transform;
                Tilemap tilemap = go.AddComponent<Tilemap>();
                TilemapRenderer tilemapRenderer = go.AddComponent<TilemapRenderer>();

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
    }
}
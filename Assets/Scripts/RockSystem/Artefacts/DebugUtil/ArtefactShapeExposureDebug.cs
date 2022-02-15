using Managers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RockSystem.Artefacts.DebugUtil
{
    public class ArtefactShapeExposureDebug : MonoBehaviour
    {
        [SerializeField] private string sortingLayer;
        [SerializeField] private int sortingOrder;
        [SerializeField] private Sprite tileSprite;
        
        private ArtefactShape artefactShape;

        private Tilemap tilemap;
        private Tile tile;

        private void Awake()
        {
            CreateTilemap();
        }

        private void Start()
        {
            artefactShape = M.GetOrThrow<ArtefactShape>();
            
            artefactShape.artefactExposed.AddListener(OnArtefactExposed);
            artefactShape.initialised.AddListener(Initialise);
        }
        
        private void Initialise()
        {
            tilemap.ClearAllTiles();
        }

        private void OnArtefactExposed()
        {
            foreach (var flatPosition in artefactShape.ChunkExposure.Keys)
            {
                tilemap.SetTile((Vector3Int)flatPosition, artefactShape.ChunkExposure[flatPosition] ? tile : null);
            }
        }
        
        private void CreateTilemap()
        {
            GameObject go = new GameObject($"Artefact Shape Exposure Debug Tilemap");
            go.transform.parent = transform;
            tilemap = go.AddComponent<Tilemap>();
            TilemapRenderer tilemapRenderer = go.AddComponent<TilemapRenderer>();

            tilemapRenderer.sortingLayerName = sortingLayer;
            tilemapRenderer.sortingOrder = sortingOrder;
            // tilemapRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            
            // Fix tile offset from grid
            tilemap.tileAnchor = Vector3.zero;
            
            tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = tileSprite;
        }

        private void OnDestroy()
        {
            artefactShape.artefactExposed.RemoveListener(OnArtefactExposed);
            artefactShape.initialised.RemoveListener(Initialise);
        }
    }
}
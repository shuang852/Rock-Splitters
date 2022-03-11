using Managers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RockSystem.Artefacts.DebugUtil
{
    public class ArtefactShapeExposureDebug : MonoBehaviour
    {
        [SerializeField] private string sortingLayer;
        [SerializeField] private int sortingOrder;
        [SerializeField] private Sprite unexposedTileSprite;
        [SerializeField] private Sprite exposedTileSprite;
        
        private ArtefactShapeManager artefactShapeManager;

        private Tilemap tilemap;
        private Tile unexposedTile;
        private Tile exposedTile;

        private void Awake()
        {
            CreateTilemap();
        }

        // TODO: Needs to be updated to support multiple ArtefactShapes
        private void Start()
        {
            artefactShapeManager = M.GetOrThrow<ArtefactShapeManager>();
            
            artefactShapeManager.artefactExposed.AddListener(OnArtefactExposed);
            artefactShapeManager.initialised.AddListener(Initialise);
        }
        
        private void Initialise()
        {
            tilemap.ClearAllTiles();
            
            OnArtefactExposed();
        }

        private void OnArtefactExposed()
        {
            foreach (var flatPosition in artefactShapeManager.MainArtefactShape.ChunkExposure.Keys)
            {
                tilemap.SetTile((Vector3Int)flatPosition, artefactShapeManager.MainArtefactShape.ChunkExposure[flatPosition] ? unexposedTile : exposedTile);
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
            
            unexposedTile = ScriptableObject.CreateInstance<Tile>();
            unexposedTile.sprite = unexposedTileSprite;
            exposedTile = ScriptableObject.CreateInstance<Tile>();
            exposedTile.sprite = exposedTileSprite;
        }

        private void OnDestroy()
        {
            artefactShapeManager.artefactExposed.RemoveListener(OnArtefactExposed);
            artefactShapeManager.initialised.RemoveListener(Initialise);
        }
    }
}
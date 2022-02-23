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
        
        private ArtefactShapeManager artefactShapeManager;

        private Tilemap tilemap;
        private Tile tile;

        private void Awake()
        {
            CreateTilemap();
        }

        // TODO: Needs to be updated to support multiple ArtefactShapes
        private void Start()
        {
            artefactShapeManager = M.GetOrThrow<ArtefactShapeManager>();
            
            artefactShapeManager.MainArtefactShape.exposed.AddListener(OnArtefactExposed);
            artefactShapeManager.MainArtefactShape.initialised.AddListener(Initialise);
        }
        
        private void Initialise()
        {
            tilemap.ClearAllTiles();
        }

        private void OnArtefactExposed()
        {
            foreach (var flatPosition in artefactShapeManager.MainArtefactShape.ChunkExposure.Keys)
            {
                tilemap.SetTile((Vector3Int)flatPosition, artefactShapeManager.MainArtefactShape.ChunkExposure[flatPosition] ? tile : null);
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
            artefactShapeManager.MainArtefactShape.exposed.RemoveListener(OnArtefactExposed);
            artefactShapeManager.MainArtefactShape.initialised.RemoveListener(Initialise);
        }
    }
}
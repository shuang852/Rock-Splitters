using UnityEngine;

namespace RockSystem.Chunks
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class RockShapeMask : MonoBehaviour
    {
        [SerializeField] private RockShape rockShape;

        private SpriteRenderer spriteRenderer;
        public PolygonCollider2D PolyCollider { get; private set; }

        public void Setup()
        {
            transform.position = rockShape.position;
            transform.rotation = Quaternion.Euler(rockShape.rotation);
            transform.localScale = rockShape.scale;
            
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            spriteRenderer.sprite = rockShape.rockShapeMask;
            PolyCollider = gameObject.AddComponent<PolygonCollider2D>();
            spriteRenderer.enabled = false;
        }
    }
}
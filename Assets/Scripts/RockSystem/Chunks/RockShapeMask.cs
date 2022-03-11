using UnityEngine;

namespace RockSystem.Chunks
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class RockShapeMask : MonoBehaviour
    {
        [SerializeField] private RockShape rockShape;

        private SpriteRenderer spriteRenderer;
        public PolygonCollider2D PolyCollider { get; private set; }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Initialise(RockShape rockShape)
        {
            var t = transform;
            
            t.localPosition = rockShape.position;
            t.rotation = Quaternion.Euler(rockShape.rotation);
            t.localScale = rockShape.scale;
            
            // Setup colliders
            spriteRenderer.sprite = rockShape.rockShapeMask;
            
            if (PolyCollider != null)
                Destroy(PolyCollider);
            
            PolyCollider = gameObject.AddComponent<PolygonCollider2D>();
            spriteRenderer.enabled = false;
        }
    }
}
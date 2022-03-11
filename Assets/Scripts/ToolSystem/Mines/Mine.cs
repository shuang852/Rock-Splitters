using Managers;
using RockSystem.Chunks;
using UnityEngine;
using UnityEngine.Events;

namespace ToolSystem.Mines
{
    public class Mine : ChunkShape
    {
        [SerializeField] private float triggerHealth;
        [SerializeField] private float defuseExposure;
        [SerializeField] private Tool tool;
        [SerializeField] private Animator animator;
        [SerializeField] private string defuseLayer;

        public UnityEvent<Mine> detonated = new UnityEvent<Mine>();
        public UnityEvent<Mine> defused = new UnityEvent<Mine>();

        private ToolManager toolManager;
        private bool hasDetonated;
        private static readonly int defuse = Animator.StringToHash("Defuse");

        protected override void Start()
        {
            base.Start();

            toolManager = M.GetOrThrow<ToolManager>();
        }

        public void Initialise(int layer)
        {
            damaged.AddListener(OnDamaged);
            
            exposed.AddListener(OnExposed);
            
            Initialise(sprite, maxHealth, layer);
        }

        private void OnDamaged()
        {
            if (Health <= triggerHealth && !hasDetonated)
                Detonate();
        }

        private void OnExposed()
        {
            if (Exposure > defuseExposure && !hasDetonated)
                Defuse();
        }

        private void Detonate()
        {
            hasDetonated = true;
            
            // Stop the mine blocking the rock underneath
            ClearChunkHealths();

            toolManager.UseTool(transform.position, tool);

            detonated.Invoke(this);
            // TODO: Quick fix. Needs looking at again.
            gameObject.SetActive(false);

            // destroyRequest.Invoke(this);
            
            // Debug.Log($"destroy request detonate {name}");
            
            // TODO: Will need an explosion animation
        }
        
        private void Defuse()
        {
            hasDetonated = true;

            SpriteRenderer.sortingLayerName = defuseLayer;
            
            animator.SetTrigger(defuse);
            defused.Invoke(this);
        }

        // Called by the end of the defuse animation
        private void Destroy()
        {
            // TODO: Should Mine destroy itself or should it ask to be destroyed by its manager? 
            // destroyRequest.Invoke(this);
            // Debug.Log($"destroy request defuse {name}");
            
            // TODO: Quick fix. Needs looking at again.
            gameObject.SetActive(false);
        }
    }
}
using Managers;
using RockSystem.Chunks;
using UnityEngine;

namespace ToolSystem.Mines
{
    public class Mine : ChunkShape
    {
        [SerializeField] private float triggerHealth;
        [SerializeField] private float defuseExposure;
        [SerializeField] private Tool tool;
        [SerializeField] private Animator animator;
        [SerializeField] private string defuseLayer;

        private ToolManager toolManager;
        private bool detonated;
        private static readonly int defuse = Animator.StringToHash("Defuse");

        protected override void Start()
        {
            base.Start();

            toolManager = M.GetOrThrow<ToolManager>();
        }

        public void Initialise(int layer)
        {
            Initialise(sprite, maxHealth, layer);
            
            damaged.AddListener(OnDamaged);
            
            exposed.AddListener(OnExposed);
        }

        private void OnDamaged()
        {
            if (Health <= triggerHealth && !detonated)
                Detonate();
        }

        private void OnExposed()
        {
            if (Exposure > defuseExposure && !detonated)
                Defuse();
        }

        private void Detonate()
        {
            detonated = true;
            
            // Stop the mine blocking the rock underneath
            ChunkHealths.Clear(); 
            
            toolManager.UseTool(transform.position, tool);

            Destroy(gameObject);
            
            // TODO: Will need an explosion animation
        }
        
        private void Defuse()
        {
            detonated = true;

            SpriteRenderer.sortingLayerName = defuseLayer;
            
            animator.SetTrigger(defuse);
        }

        private void Destroy()
        {
            // TODO: Should Mine destroy itself or should it ask to be destroyed by its manager? 
            Destroy(gameObject);
        }
    }
}
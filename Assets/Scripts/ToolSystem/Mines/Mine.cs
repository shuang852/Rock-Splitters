using Managers;
using RockSystem.Chunks;
using UnityEngine;

namespace ToolSystem.Mines
{
    public class Mine : ChunkShape
    {
        [SerializeField] private float triggerHealth;
        [SerializeField] private Tool tool;

        private ToolManager toolManager;
        private bool detonated;

        protected override void Start()
        {
            base.Start();

            toolManager = M.GetOrThrow<ToolManager>();
        }

        public void Initialise(int layer)
        {
            Initialise(sprite, maxHealth, layer);
            
            damaged.AddListener(OnDamaged);
        }

        private void OnDamaged()
        {
            if (Health <= triggerHealth && !detonated)
                Detonate();
        }

        private void Detonate()
        {
            detonated = true;
            
            toolManager.UseTool(transform.position, tool);

            Destroy(gameObject);
            
            // TODO: Will need an explosion animation
        }
    }
}
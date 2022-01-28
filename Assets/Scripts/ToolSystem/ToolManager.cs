using System.Collections.Generic;
using Managers;
using RockSystem.Chunks;
using UnityEngine;
using UnityEngine.Events;

namespace ToolSystem
{
    public class ToolManager : Manager
    {
        //[SerializeField] private List<Tool> tools;

        private ChunkManager chunkManager;

       // private int currentToolIndex = 0;

       [SerializeField] private Tool startingTool;

        public Tool CurrentTool { get; private set; }

        // TODO: Better naming
        public UnityEvent<Vector2> eToolDown = new UnityEvent<Vector2>();
        public UnityEvent<Vector2> eToolInUse = new UnityEvent<Vector2>();
        public UnityEvent<Vector2> eToolUp = new UnityEvent<Vector2>();

        protected override void Start()
        {
            base.Start();
            
            chunkManager = M.GetOrThrow<ChunkManager>();
        }

        /// <summary>
        /// Called once when the tool starts being used. Similar to <c>Input.GetKeyDown</c>.
        /// </summary>
        public void ToolDown(Vector2 worldPosition)
        {
            if (CurrentTool.action == Tool.ToolAction.Tap)
            {
                UseTool(worldPosition);
                
                eToolDown.Invoke(worldPosition);
            }
        }

        /// <summary>
        /// Called each frame the tool is being used including the first. Similar to <c>Input.GetKey</c>.
        /// </summary>
        /// <param name="worldPosition"></param>
        public void ToolInUse(Vector2 worldPosition)
        {
            if (CurrentTool.action == Tool.ToolAction.Continuous)
            {
                UseTool(worldPosition);
                
                eToolInUse.Invoke(worldPosition);
            }
        }

        /// <summary>
        /// Called once when the tool stops being used. Similar to <c>Input.GetKeyUp</c>.
        /// </summary>
        public void ToolUp(Vector2 worldPosition)
        {
            eToolUp.Invoke(worldPosition);
        }

        // TODO: Can be more efficient. Pass the function instead of looping through the chunks again.
        private void UseTool(Vector2 worldPosition)
        {
            List<ChunkManager.OddrChunkCoord> affectedChunks = chunkManager.GetChunksInRadius(worldPosition, CurrentTool.radius);

            bool willDamageFossil = !(CurrentTool.artefactSafety && chunkManager.WillDamageRock(affectedChunks));

            foreach (var affectedChunk in affectedChunks)
            {
                float normalisedDistance =
                    Vector2.Distance(chunkManager.GetChunkWorldPosition(affectedChunk), worldPosition) /
                    CurrentTool.radius;

                int calculatedDamage =
                    Mathf.CeilToInt(CurrentTool.damageFalloff.Evaluate(normalisedDistance) * CurrentTool.damage);
                
                int clampedDamage = Mathf.Clamp(calculatedDamage, 0, CurrentTool.damage);

                chunkManager.DamageChunk(affectedChunk, clampedDamage, willDamageFossil);
            }
        }

        public void SelectTool(Tool tool)
        {
            CurrentTool = tool;
        }
    }
}

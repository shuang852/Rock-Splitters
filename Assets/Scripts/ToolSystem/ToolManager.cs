using System.Collections.Generic;
using Managers;
using RockSystem;
using UnityEngine;

namespace ToolSystem
{
    public class ToolManager : Manager
    {
        private ChunkManager chunkManager;
        
        public Tool CurrentTool { get; private set; }

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
                UseTool(worldPosition);
        }

        /// <summary>
        /// Called each frame the tool is being used including the first. Similar to <c>Input.GetKey</c>.
        /// </summary>
        /// <param name="worldPosition"></param>
        public void ToolInUse(Vector2 worldPosition)
        {
            if (CurrentTool.action == Tool.ToolAction.Continuous)
                UseTool(worldPosition);
        }

        /// <summary>
        /// Called once when the tool stops being used. Similar to <c>Input.GetKeyUp</c>.
        /// </summary>
        public void ToolUp(Vector2 worldPosition) {}

        private void UseTool(Vector2 worldPosition)
        {
            List<Vector2Int> affectedChunks = chunkManager.GetChunksInRadius(worldPosition, CurrentTool.radius);

            foreach (var affectedChunk in affectedChunks)
            {
                chunkManager.DamageChunk(affectedChunk, CurrentTool.damage);
            }
        }

        public void SelectTool(Tool tool) => CurrentTool = tool;
    }
}

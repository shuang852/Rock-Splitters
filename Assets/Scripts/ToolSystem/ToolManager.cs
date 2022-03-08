using System.Collections.Generic;
using Managers;
using RockSystem.Artefacts;
using RockSystem.Chunks;
using ToolSystem.Mines;
using UnityEngine;
using UnityEngine.Events;
using Utility;

namespace ToolSystem
{
    public class ToolManager : Manager
    {
        private ChunkManager chunkManager;
        private ArtefactShapeManager artefactShapeManager;
        private MineManager mineManager;
        
        public Tool CurrentTool { get; private set; }

        public UnityEvent<Vector2> toolDown = new UnityEvent<Vector2>();
        public UnityEvent<Vector2> toolInUse = new UnityEvent<Vector2>();
        public UnityEvent<Vector2> toolUp = new UnityEvent<Vector2>();
        public UnityEvent toolUsed = new UnityEvent();
        
        protected override void Start()
        {
            base.Start();
            
            chunkManager = M.GetOrThrow<ChunkManager>();
            artefactShapeManager = M.GetOrThrow<ArtefactShapeManager>();
            mineManager = M.GetOrThrow<MineManager>();
        }

        /// <summary>
        /// Called once when the tool starts being used. Similar to <c>Input.GetKeyDown</c>.
        /// </summary>
        public void ToolDown(Vector2 worldPosition)
        {
            if (!CurrentTool)
                return;
            
            if (CurrentTool.action == Tool.ToolAction.Tap)
            {
                UseTool(worldPosition);
                
                toolDown.Invoke(worldPosition);
            }
        }

        /// <summary>
        /// Called each frame the tool is being used including the first. Similar to <c>Input.GetKey</c>.
        /// </summary>
        /// <param name="worldPosition"></param>
        public void ToolInUse(Vector2 worldPosition)
        {
            if (!CurrentTool)
                return;
            
            if (CurrentTool.action == Tool.ToolAction.Continuous)
            {
                UseTool(worldPosition);
                
                toolInUse.Invoke(worldPosition);
            }
        }

        /// <summary>
        /// Called once when the tool stops being used. Similar to <c>Input.GetKeyUp</c>.
        /// </summary>
        public void ToolUp(Vector2 worldPosition)
        {
            toolUp.Invoke(worldPosition);
        }

        // TODO: Can be more efficient. Pass the function instead of looping through the chunks again.
        public void UseTool(Vector2 worldPosition, Tool tool = null)
        {
            if (tool == null)
                tool = CurrentTool;

            List<Hexagons.OddrChunkCoord> affectedChunks = Hexagons.GetChunksInRadius(chunkManager.CurrentGrid, worldPosition, tool.radius);

            bool willDamageRock = chunkManager.WillDamageRock(affectedChunks);

            artefactShapeManager.ChunkShapesCanBeDamaged = !(tool.artefactSafety && willDamageRock);
            mineManager.ChunkShapesCanBeDamaged = !(tool.mineSafety && willDamageRock);

            foreach (var affectedChunk in affectedChunks)
            {
                float normalisedDistance =
                    Vector2.Distance(chunkManager.GetChunkWorldPosition(affectedChunk), worldPosition) /
                    tool.radius;

                float calculatedDamage = tool.damageFalloff.Evaluate(normalisedDistance) * tool.damage;
                
                float clampedDamage = Mathf.Clamp(calculatedDamage, 0, tool.damage);
                
                if (tool.action == Tool.ToolAction.Continuous)
                    clampedDamage *= Time.deltaTime;

                chunkManager.DamageChunksAtPosition(affectedChunk, clampedDamage);
            }

            artefactShapeManager.ChunkShapesCanBeDamaged = true;
            mineManager.ChunkShapesCanBeDamaged = true;
            
            artefactShapeManager.CheckHealthAndExposure();
            mineManager.CheckHealthAndExposure();
            
            toolUsed.Invoke();
        }

        public void SelectTool(Tool tool) => CurrentTool = tool;
    }
}

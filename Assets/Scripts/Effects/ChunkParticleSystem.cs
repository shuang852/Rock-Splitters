using Cleaning;
using Managers;
using RockSystem.Chunks;
using ToolSystem;
using UnityEngine;

namespace Effects
{
    public class ChunkParticleSystem : MonoBehaviour
    {
        private ChunkManager chunkManager;
        private ToolManager toolManager;
        private CleaningManager cleaningManager;

        private Grid currentGrid;
        private ParticleSystem particleSystem;

        private void Start()
        {
            chunkManager = M.GetOrThrow<ChunkManager>();
            toolManager = M.GetOrThrow<ToolManager>();
            particleSystem = GetComponent<ParticleSystem>();
            cleaningManager = M.GetOrThrow<CleaningManager>();

            currentGrid = chunkManager.CurrentGrid;
            cleaningManager.nextArtefactRockGenerated.AddListener(ChunkRegistered);
            chunkManager.chunkCleared.AddListener(PSPlay);
        }

        private void ChunkRegistered()
        {
            // TODO: Change the colour of the particle to have more contrast
            var main = particleSystem.main;
            var color = cleaningManager.CurrentArtefactRock.RockColor;
            // color = new Color(color.r - offset, color.g -offset,
            //     color.b + -offset, 1);
            color.a = 0.9f;
            main.startColor = color;
        }

        private void PSPlay(Chunk chunk)
        {
            if (toolManager.CurrentTool.action != Tool.ToolAction.Continuous) return;

            gameObject.transform.position = currentGrid.CellToWorld(chunk.Position);
            particleSystem.Play();
        }
    }
}
using System;
using Effects;
using Managers;
using UnityEngine;

namespace ToolSystem
{
    /*
     * Creates the visuals when using a tool. Drill is a toggle on and off while hammer we instantiate a GO
     */
    public class ToolVisuals : MonoBehaviour
    {
        [SerializeField] private ParticleSystem drillParticles;
        [SerializeField] private GameObject hammerVisPrefab;
        [SerializeField] private Animator drillAnimator;
        [SerializeField] private CameraShake cameraShake; 
        private ToolManager toolManager;
        
        private bool toolInUse;
        private static readonly int cleaning = Animator.StringToHash("Cleaning");

        private void Start()
        {
            toolManager = M.GetOrThrow<ToolManager>();
            
            toolManager.toolDown.AddListener(OnToolDown);
            toolManager.toolInUse.AddListener(OnToolInUse);
            toolManager.toolUp.AddListener(OnToolUp);
        }

        private void OnToolDown(Vector2 worldPosition)
        {
            Clean(worldPosition);
        }

        private void OnToolInUse(Vector2 worldPosition)
        {
            Clean(worldPosition);
        }

        private void OnToolUp(Vector2 worldPosition)
        {
            StopClean();
        }

        /// <summary>
        /// Activates the tools particle and visual system based on current tool
        /// For tap it's a one shot particles. For continuous its continuous.
        /// </summary>
        /// <param name="position">Position of where the particles to be played</param>
        /// <exception cref="ArgumentOutOfRangeException">Unknown tool</exception>
        private void Clean(Vector3 position)
        {
            Tool.ToolAction toolAction = toolManager.CurrentTool.action;
                
            transform.position = position;
            switch (toolAction)
            {
                case Tool.ToolAction.Tap:
                    Instantiate(hammerVisPrefab, transform);
                    cameraShake.Shake();
                    break;
                case Tool.ToolAction.Continuous:
                    if (!toolInUse)
                    {
                        drillAnimator.SetBool(cleaning, true);
                        if (!drillParticles.isEmitting) drillParticles.Play();
                        toolInUse = true;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(toolAction), toolAction, null);
            }
        }

        private void StopClean()
        {
            drillAnimator.SetBool(cleaning, false);
            toolInUse = false;
            drillParticles.Stop();
        }

        private void OnDestroy()
        {
            toolManager.toolDown.RemoveListener(OnToolDown);
            toolManager.toolInUse.RemoveListener(OnToolInUse);
            toolManager.toolUp.RemoveListener(OnToolUp);
        }
    }
}
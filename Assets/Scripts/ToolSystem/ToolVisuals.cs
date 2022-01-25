using System;
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
        private ToolManager toolManager;

        private static readonly int cleaning = Animator.StringToHash("Cleaning");

        private void Start() => toolManager = M.GetOrThrow<ToolManager>();

        public void Clean(Tool.ToolAction toolAction, Vector3 position)
        {
            transform.position = position;
            switch (toolAction)
            {
                case Tool.ToolAction.Tap:
                    Instantiate(hammerVisPrefab, transform);
                    break;
                case Tool.ToolAction.Continuous:
                    drillAnimator.SetBool(cleaning, true);
                    if (!drillParticles.isEmitting) drillParticles.Play();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(toolAction), toolAction, null);
            }
        }
    
        public void StopClean()
        {
            // TODO: Replace this once UI takes priority over touch input
            drillAnimator.SetBool(cleaning, false);
            drillParticles.Stop(); 
            
            //var tool = toolManager.CurrentTool;
            
            // switch (tool.action)
            // {
            //     // Change to switch if there are different or tap requires extra work
            //     case Tool.ToolAction.Continuous:
            //         drillAnimator.SetBool(cleaning, false);
            //         drillParticles.Stop();
            //         break;
            //     case Tool.ToolAction.Tap:
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException();
            // }
        }
    }
}
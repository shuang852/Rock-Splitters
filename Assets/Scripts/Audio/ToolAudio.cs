using System;
using Managers;
using ToolSystem;
using UnityEngine;

namespace Audio
{
    public class ToolAudio : MonoBehaviour
    {
        [SerializeField] private PlayOneShot drill;
        [SerializeField] private PlayOneShot hammer;
        
        private ToolManager toolManager;
        private bool toolInUse;
        
        // Start is called before the first frame update
        private void Start()
        {
            toolManager = M.GetOrThrow<ToolManager>();
            toolManager.toolDown.AddListener(OnToolUse);
            toolManager.toolInUse.AddListener(OnToolUse);
            toolManager.toolUp.AddListener(OnToolStop);
        }

        private void OnToolUse(Vector2 worldPosition)
        {
            if (toolManager.CurrentTool == null) return;
            
            Tool.ToolAction toolAction = toolManager.CurrentTool.action;
            
            switch (toolAction)
            {
                case Tool.ToolAction.Tap:
                    hammer.PlayOnce();
                    break;
                case Tool.ToolAction.Continuous:
                    if (!toolInUse)
                    {
                        toolInUse = true;
                        drill.PlayOnce();
                        FMODUnity.RuntimeManager.StudioSystem.setParameterByNameWithLabel("Drill", "Drilling");
                    }
                    break;
            }
        }

        private void OnToolStop(Vector2 worldPosition)
        {
            toolInUse = false;
            FMODUnity.RuntimeManager.StudioSystem.setParameterByNameWithLabel("Drill", "Not Drilling");
        }

        private void OnDisable()
        {
            drill.StopImmediate();
        }
    }
}

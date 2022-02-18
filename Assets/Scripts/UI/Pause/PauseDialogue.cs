using System;
using Cleaning;
using Managers;
using UI.Core;
using UnityEngine;

namespace UI.Pause
{
    public class PauseDialogue : Dialogue
    {
        public Action Abandoned;

        private CleaningManager cleaningManager;
        private CleaningTimerManager timerManager;
        private ToolManager toolManager;

        protected override void OnAwake() => Abandoned += OnAbandoned;
        private Tool previousTool;

        protected override void OnClose()
        {
            cleaningManager.ResumeCleaning();
        
        }
        protected override void OnPromote()
        {
            cleaningManager = M.GetOrThrow<CleaningManager>();
        
            cleaningManager.PauseCleaning();
        }
        protected override void OnDemote() { }

        private void OnAbandoned() => canvasGroup.interactable = false;
    }
        protected override void OnAwake() => Abandoned += OnAbandoned;

        protected override void OnClose()
        {
            timerManager.StartTimer();
            Debug.Log(previousTool);
            toolManager.SelectTool(previousTool);
        }
        protected override void OnPromote()
        {
            timerManager = M.GetOrThrow<CleaningTimerManager>();
            toolManager = M.GetOrThrow<ToolManager>();

            // Prevents coming back to and from pause to recall these functions
            if (previousTool != null) return;
            
            timerManager.StopTimer();
            previousTool = toolManager.CurrentTool;
            toolManager.SelectTool(null);
        }
        protected override void OnDemote() { }

        private void OnAbandoned() => canvasGroup.interactable = false;
    }
}

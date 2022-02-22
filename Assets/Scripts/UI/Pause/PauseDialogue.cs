using System;
using Cleaning;
using Managers;
using ToolSystem;
using UI.Core;
using UnityEngine;

namespace UI.Pause
{
    public class PauseDialogue : Dialogue
    {
        public Action Abandoned;

        [SerializeField] private GameObject blurBackground;

        private bool opened;
        private CleaningManager cleaningManager;
        private CleaningTimerManager timerManager;
        private ToolManager toolManager;

        protected override void OnAwake() => Abandoned += OnAbandoned;
        private Tool previousTool;

        protected override void OnClose()
        {
            timerManager.StartTimer();
            cleaningManager.ResumeCleaning();
            toolManager.SelectTool(previousTool);
            opened = false;
        }
        
        protected override void OnPromote()
        {
            cleaningManager = M.GetOrThrow<CleaningManager>();
            timerManager = M.GetOrThrow<CleaningTimerManager>();
            toolManager = M.GetOrThrow<ToolManager>();
            
            if (!blurBackground.activeSelf) blurBackground.SetActive(true);

            // Only call these functions once
            if (opened) return;

            timerManager.StopTimer();
            previousTool = toolManager.CurrentTool;
            Debug.Log(previousTool);
            toolManager.SelectTool(null);
        
            cleaningManager.PauseCleaning();
            opened = true;
        }

        protected override void OnDemote()
        {
            blurBackground.SetActive(false);
        }

        private void OnAbandoned() => canvasGroup.interactable = false;
    }
}

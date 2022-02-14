using System;
using Cleaning;
using Managers;
using UI.Core;

namespace UI.Pause
{
    public class PauseDialogue : Dialogue
    {
        public Action Abandoned;

        private CleaningManager cleaningManager;

        protected override void OnAwake() => Abandoned += OnAbandoned;

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
}

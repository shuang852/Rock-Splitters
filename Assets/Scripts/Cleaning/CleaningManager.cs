using Managers;
using UnityEngine.Events;

namespace Cleaning
{
    public class CleaningManager : Manager
    {
        enum CleaningState
        {
            InProgress,
            Won,
            Lost
        }
        
        private CleaningState cleaningState;

        public UnityEvent CleaningStarted = new UnityEvent();
        public UnityEvent CleaningWon = new UnityEvent();
        public UnityEvent CleaningLost = new UnityEvent();

        protected override void Start()
        {
            base.Start();
            
            // TODO: Starting cleaning here creates a race condition.
            // StartCleaning();
        }

        public void StartCleaning()
        {
            cleaningState = CleaningState.InProgress;
            
            CleaningStarted.Invoke();    
        }

        public void LoseCleaning()
        {
            cleaningState = CleaningState.Lost;
            
            CleaningLost.Invoke();
        }

        public void WinCleaning()
        {
            cleaningState = CleaningState.Won;
            
            CleaningWon.Invoke();
        }
    }
}
using Managers;
using UnityEngine.Events;

namespace Cleaning
{
    public class CleaningManager : Manager
    {
        public enum CleaningState
        {
            InProgress,
            Won,
            Lost
        }

        public CleaningState CurrentCleaningState { get; private set; }

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
            CurrentCleaningState = CleaningState.InProgress;
            
            CleaningStarted.Invoke();    
        }

        public void LoseCleaning()
        {
            CurrentCleaningState = CleaningState.Lost;
            
            CleaningLost.Invoke();
        }

        public void WinCleaning()
        {
            CurrentCleaningState = CleaningState.Won;
            
            CleaningWon.Invoke();
        }
    }
}
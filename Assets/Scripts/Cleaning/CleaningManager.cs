using Managers;
using RockSystem.Chunks;
using RockSystem.Fossils;
using UnityEngine;
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

        [SerializeField] private float RequiredExposureForCompletion;
        [SerializeField] private float RequiredHealthForFailure;
 
        public CleaningState CurrentCleaningState { get; private set; }

        public UnityEvent CleaningStarted = new UnityEvent();
        public UnityEvent CleaningEnded = new UnityEvent();
        public UnityEvent CleaningWon = new UnityEvent();
        public UnityEvent CleaningLost = new UnityEvent();

        private ChunkManager chunkManager;
        private FossilShape fossilShape;

        protected override void Start()
        {
            base.Start();
            
            // TODO: Starting cleaning here creates a race condition.
            // StartCleaning();

            chunkManager = M.GetOrThrow<ChunkManager>();
            fossilShape = M.GetOrThrow<FossilShape>();
        }

        public void StartCleaning()
        {
            CurrentCleaningState = CleaningState.InProgress;
            
            fossilShape.fossilExposed.AddListener(CheckIfCleaningWon);
            fossilShape.fossilDamaged.AddListener(CheckIfCleaningLost);
            
            CleaningStarted.Invoke();    
        }
        
        private void EndCleaning()
        {
            fossilShape.fossilExposed.RemoveListener(CheckIfCleaningWon);
            fossilShape.fossilDamaged.RemoveListener(CheckIfCleaningLost);
            
            CleaningEnded.Invoke();
        }

        public void LoseCleaning()
        {
            CurrentCleaningState = CleaningState.Lost;
            
            EndCleaning();
            
            CleaningLost.Invoke();
        }

        public void WinCleaning()
        {
            CurrentCleaningState = CleaningState.Won;
            
            chunkManager.HideRock();
            
            EndCleaning();
            
            CleaningWon.Invoke();
        }

        public void CheckIfCleaningLost()
        {
            if (fossilShape.FossilHealth < RequiredHealthForFailure)
                LoseCleaning();
        }

        public void CheckIfCleaningWon()
        {
            if (fossilShape.FossilExposure > RequiredExposureForCompletion)
                WinCleaning();
        }
    }
}
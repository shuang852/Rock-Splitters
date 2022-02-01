using Managers;
using RockSystem.Fossils;
using ToolSystem;
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
        public UnityEvent CleaningWon = new UnityEvent();
        public UnityEvent CleaningLost = new UnityEvent();
        
        private ToolManager toolManager;
        private FossilShape fossilShape;

        protected override void Start()
        {
            base.Start();
            
            // TODO: Starting cleaning here creates a race condition.
            // StartCleaning();
            
            toolManager = M.GetOrThrow<ToolManager>();
            fossilShape = M.GetOrThrow<FossilShape>();
            
            toolManager.toolUsed.AddListener(CheckIfCleaningWon);
            fossilShape.fossilDamaged.AddListener(CheckIfCleaningLost);
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

        public void CheckIfCleaningLost()
        {
            if (fossilShape.FossilHealth() < RequiredHealthForFailure)
                LoseCleaning();
        }

        public void CheckIfCleaningWon()
        {
            if (fossilShape.FossilExposure() > RequiredExposureForCompletion)
                WinCleaning();
        }
    }
}
using System.Linq;
using Managers;
using RockSystem.Artefacts;
using RockSystem.Chunks;
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

        [SerializeField] private GenerationBracket generationBracket;
        [SerializeField] private float requiredExposureForCompletion;
        [SerializeField] private float requiredHealthForFailure;
 
        public CleaningState CurrentCleaningState { get; private set; }

        public UnityEvent cleaningStarted = new UnityEvent();
        public UnityEvent cleaningEnded = new UnityEvent();
        public UnityEvent cleaningWon = new UnityEvent();
        public UnityEvent cleaningLost = new UnityEvent();

        private ChunkManager chunkManager;
        private ArtefactShape artefactShape;

        public ArtefactRock CurrentArtefactRock { get; private set; }

        protected override void Start()
        {
            base.Start();
            
            // TODO: Starting cleaning here creates a race condition.
            // StartCleaning();

            chunkManager = M.GetOrThrow<ChunkManager>();
            artefactShape = M.GetOrThrow<ArtefactShape>();
        }

        public void StartCleaning()
        {
            CurrentCleaningState = CleaningState.InProgress;
            
            artefactShape.artefactExposed.AddListener(CheckIfCleaningWon);
            artefactShape.artefactDamaged.AddListener(CheckIfCleaningLost);

            CurrentArtefactRock = GenerateArtefactRock(generationBracket);
            chunkManager.Initialise(CurrentArtefactRock.RockShape, CurrentArtefactRock.RockColor, CurrentArtefactRock.ChunkDescription);
            artefactShape.Initialise(CurrentArtefactRock.Artefact);
            
            cleaningStarted.Invoke();
        }

        public ArtefactRock GenerateArtefactRock(GenerationBracket generationBracket)
        {
            var artefacts = generationBracket.artefacts;
            var rockShapes = generationBracket.rockShapes;
            var chunkDescriptions = generationBracket.chunkDescriptions;
            
            return new ArtefactRock(
                artefacts.ElementAtOrDefault(Random.Range(0, artefacts.Count)),
                rockShapes.ElementAtOrDefault(Random.Range(0, rockShapes.Count)),
                chunkDescriptions.ElementAtOrDefault(Random.Range(0, chunkDescriptions.Count)),
                generationBracket.rockColor
            );
        }
        
        private void EndCleaning()
        {
            artefactShape.artefactExposed.RemoveListener(CheckIfCleaningWon);
            artefactShape.artefactDamaged.RemoveListener(CheckIfCleaningLost);
            
            cleaningEnded.Invoke();
        }

        public void LoseCleaning()
        {
            CurrentCleaningState = CleaningState.Lost;
            
            EndCleaning();
            
            cleaningLost.Invoke();
        }

        public void WinCleaning()
        {
            CurrentCleaningState = CleaningState.Won;
            
            chunkManager.HideRock();
            
            EndCleaning();
            
            cleaningWon.Invoke();
        }

        public void CheckIfCleaningLost()
        {
            if (artefactShape.ArtefactHealth < requiredHealthForFailure)
                LoseCleaning();
        }

        public void CheckIfCleaningWon()
        {
            if (artefactShape.ArtefactExposure > requiredExposureForCompletion)
                WinCleaning();
        }
    }
}
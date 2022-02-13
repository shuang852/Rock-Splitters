using System.Collections.Generic;
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
            Finished
        }

        [SerializeField] private List<GenerationBracket> generationBrackets = new List<GenerationBracket>();
        [SerializeField] private float requiredExposureForCompletion;
        [SerializeField] private float requiredHealthForFailure;
 
        public CleaningState CurrentCleaningState { get; private set; }

        public UnityEvent cleaningStarted = new UnityEvent();
        public UnityEvent cleaningEnded = new UnityEvent();
        public UnityEvent nextArtefactRock = new UnityEvent();
        public UnityEvent artefactRockFailed = new UnityEvent();
        public UnityEvent artefactRockSucceeded = new UnityEvent();

        private ChunkManager chunkManager;
        private ArtefactShape artefactShape;

        private int currentGenerationBracketIndex;
        private GenerationBracket CurrentGenerationBracket => generationBrackets[currentGenerationBracketIndex];
        private int artefactsCleaned;
        private int artefactsCleanedInBracket;
        private int artefactsCleanedSuccessfully;

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
            
            artefactShape.artefactExposed.AddListener(CheckIfArtefactRockSucceeded);
            artefactShape.artefactDamaged.AddListener(CheckIfArtefactRockFailed);

            currentGenerationBracketIndex = 0;
            
            cleaningStarted.Invoke();

            NextArtefactRock();
        }

        public void NextArtefactRock()
        {
            if (artefactsCleanedInBracket >= CurrentGenerationBracket.bracketLength &&
                currentGenerationBracketIndex < generationBrackets.Count - 1)
            {
                currentGenerationBracketIndex++;
                artefactsCleanedInBracket = 0;
            }
            
            CurrentArtefactRock = GenerateArtefactRock(CurrentGenerationBracket);
            
            chunkManager.Initialise(CurrentArtefactRock.RockShape, CurrentArtefactRock.RockColor, CurrentArtefactRock.ChunkDescription);
            artefactShape.Initialise(CurrentArtefactRock.Artefact);
            
            nextArtefactRock.Invoke();
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

        public void EndCleaning()
        {
            CurrentCleaningState = CleaningState.Finished;
            
            artefactShape.artefactExposed.RemoveListener(CheckIfArtefactRockSucceeded);
            artefactShape.artefactDamaged.RemoveListener(CheckIfArtefactRockFailed);
            
            cleaningEnded.Invoke();
        }

        public void ArtefactRockFailed()
        {
            artefactsCleaned++;
            artefactsCleanedInBracket++;
            
            artefactRockFailed.Invoke();
            
            NextArtefactRock();
        }

        public void ArtefactRockSucceeded()
        {
            artefactsCleaned++;
            artefactsCleanedInBracket++;
            artefactsCleanedSuccessfully++;
            
            artefactRockSucceeded.Invoke();
            
            NextArtefactRock();
            
            // TODO: Where should this go?
            // chunkManager.HideRock();
        }

        private void CheckIfArtefactRockFailed()
        {
            if (artefactShape.ArtefactHealth < requiredHealthForFailure)
                ArtefactRockFailed();
        }

        private void CheckIfArtefactRockSucceeded()
        {
            if (artefactShape.ArtefactExposure > requiredExposureForCompletion)
                ArtefactRockSucceeded();
        }
    }
}
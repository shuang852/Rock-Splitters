using System.Collections.Generic;
using System.Linq;
using Managers;
using RockSystem.Artefacts;
using RockSystem.Chunks;
using ToolSystem;
using Stored;
using ToolSystem.Mines;
using UnityEngine;
using UnityEngine.Events;

namespace Cleaning
{
    public class CleaningManager : Manager
    {
        public enum CleaningState
        {
            InProgress,
            Succeeded,
            Failed,
            Finished
        }

        [SerializeField] private List<GenerationBracket> generationBrackets = new List<GenerationBracket>();
        [SerializeField] private float requiredExposureForCompletion;
        [SerializeField] private float requiredHealthForFailure;
 
        public CleaningState CurrentCleaningState { get; private set; }

        public UnityEvent cleaningStarted = new UnityEvent();
        public UnityEvent cleaningPaused = new UnityEvent();
        public UnityEvent cleaningResumed = new UnityEvent();
        public UnityEvent cleaningEnded = new UnityEvent();
        public UnityEvent nextArtefactRockGenerated = new UnityEvent();
        public UnityEvent nextArtefactRockStarted = new UnityEvent();
        public UnityEvent artefactRockFailed = new UnityEvent();
        public UnityEvent artefactRockSucceeded = new UnityEvent();
        public UnityEvent artefactRockCompleted = new UnityEvent();
        public UnityEvent artefactStatsCompleted = new UnityEvent();

        private ChunkManager chunkManager;
        private ArtefactShapeManager artefactShapeManager;
        private ToolManager toolManager;
        private ArtefactManager artefactManager;
        private MineManager mineManager;

        private int currentGenerationBracketIndex;
        private GenerationBracket CurrentGenerationBracket => generationBrackets[currentGenerationBracketIndex];
        private int artefactsCleaned;
        private int artefactsCleanedInBracket;
        private int artefactsCleanedSuccessfully;
        private int artefactsCleanedFailed;
        private Tool previousTool;

        public ArtefactRock CurrentArtefactRock { get; private set; }

        protected override void Start()
        {
            base.Start();
            
            // TODO: Starting cleaning here creates a race condition.
            // StartCleaning();

            chunkManager = M.GetOrThrow<ChunkManager>();
            artefactShapeManager = M.GetOrThrow<ArtefactShapeManager>();
            toolManager = M.GetOrThrow<ToolManager>();
            artefactManager = M.GetOrThrow<ArtefactManager>();
            mineManager = M.GetOrThrow<MineManager>();
        }

        public void StartCleaning()
        {
            CurrentCleaningState = CleaningState.InProgress;
            
            // artefactShapeManager.artefactExposed.AddListener(CheckIfArtefactRockSucceeded);
            // artefactShapeManager.artefactDamaged.AddListener(CheckIfArtefactRockFailed);
            
            // TODO: Prevents end checks being called more than once per frame. Quick solve, should be checked.
            toolManager.toolDown.AddListener(OnToolDownOrInUse);
            toolManager.toolInUse.AddListener(OnToolDownOrInUse);
            toolManager.toolDown.AddListener(OnToolDownOrInUse);
            toolManager.toolInUse.AddListener(OnToolDownOrInUse);
            
            nextArtefactRockStarted.AddListener(OnNextArtefactRockStarted);

            currentGenerationBracketIndex = 0;

            cleaningStarted.Invoke();
            NextArtefactRock();
        }

        private void OnNextArtefactRockStarted()
        {
            CurrentCleaningState = CleaningState.InProgress;
        }

        private void OnToolDownOrInUse(Vector2 arg0)
        {
            CheckIfArtefactRockSucceeded();
            CheckIfArtefactRockFailed();
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
            artefactShapeManager.Initialise(CurrentArtefactRock.Artefact);
            mineManager.Initialise(Random.Range(CurrentGenerationBracket.minMines, CurrentGenerationBracket.maxMines + 1));
            
            nextArtefactRockGenerated.Invoke();
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

        [ContextMenu("Force end")]
        public void EndCleaning()
        {
            CurrentCleaningState = CleaningState.Finished;
            
            // artefactShapeManager.artefactExposed.RemoveListener(CheckIfArtefactRockSucceeded);
            // artefactShapeManager.artefactDamaged.RemoveListener(CheckIfArtefactRockFailed);
            
            toolManager.toolDown.RemoveListener(OnToolDownOrInUse);
            toolManager.toolInUse.RemoveListener(OnToolDownOrInUse);
            toolManager.toolDown.RemoveListener(OnToolDownOrInUse);
            toolManager.toolInUse.RemoveListener(OnToolDownOrInUse);
            
            nextArtefactRockStarted.RemoveListener(OnNextArtefactRockStarted);
            
            cleaningEnded.Invoke();
        }

        // TODO: Duplicate code. See ArtefactRockSucceeded.
        public void ArtefactRockFailed()
        {
            CurrentCleaningState = CleaningState.Failed;
            
            PauseCleaning();
            artefactsCleaned++;
            artefactsCleanedInBracket++;
            artefactsCleanedFailed++;
            
            artefactRockFailed.Invoke();
            artefactRockCompleted.Invoke();
            artefactStatsCompleted.Invoke();
            
            //NextArtefactRock();
        }

        // TODO: Duplicate code. See ArtefactRockFailed.
        public void ArtefactRockSucceeded()
        {
            CurrentCleaningState = CleaningState.Succeeded;
            
            PauseCleaning();
            
            artefactsCleaned++;
            artefactsCleanedInBracket++;
            artefactsCleanedSuccessfully++;

            artefactManager.AddItem(artefactShapeManager.MainArtefactShape.Artefact);
            artefactRockSucceeded.Invoke();
            artefactRockCompleted.Invoke();
            artefactStatsCompleted.Invoke();

            //NextArtefactRock();
            
            // TODO: Where should this go?
            // chunkManager.HideRock();
        }

        private void CheckIfArtefactRockFailed()
        {
            if (CurrentCleaningState == CleaningState.InProgress && artefactShapeManager.Health < requiredHealthForFailure)
                ArtefactRockFailed();
        }

        private void CheckIfArtefactRockSucceeded()
        {
            if (CurrentCleaningState == CleaningState.InProgress && artefactShapeManager.Exposure > requiredExposureForCompletion)
                ArtefactRockSucceeded();
        }

        public void PauseCleaning()
        {
            toolManager.toolUp.Invoke(Vector2.zero);
            previousTool = toolManager.CurrentTool;
            toolManager.SelectTool(null);
            
            cleaningPaused.Invoke();
        }

        public void ResumeCleaning()
        {
            toolManager.SelectTool(previousTool);

            cleaningResumed.Invoke();
        }
    }
}
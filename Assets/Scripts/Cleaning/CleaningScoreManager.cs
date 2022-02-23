using Managers;
using RockSystem.Artefacts;
using UnityEngine;
using UnityEngine.Events;

namespace Cleaning
{
    public class CleaningScoreManager : Manager
    {
        [SerializeField] private float requiredArtefactExposureForScoring;
        private CleaningManager cleaningManager;
        private ArtefactShapeManager artefactShapeManager;
        
        public UnityEvent scoreUpdated = new UnityEvent();

        public float Score { get; private set; }

        protected override void Start()
        {
            base.Start();
            
            cleaningManager = M.GetOrThrow<CleaningManager>();
            artefactShapeManager = M.GetOrThrow<ArtefactShapeManager>();

            cleaningManager.cleaningStarted.AddListener(ResetScore);
            cleaningManager.artefactRockSucceeded.AddListener(UpdateScore);
            cleaningManager.cleaningEnded.AddListener(UpdateScore);
        }

        private void ResetScore()
        {
            Score = 0;
        }

        private void UpdateScore()
        {
            if (!(artefactShapeManager.MainArtefactShape.Exposure >= requiredArtefactExposureForScoring)) return;
            
            // TODO: Incorporate rock difficulty.
            // TODO: Final score = Base * Health * Cleanliness * Rock Diff
            var artefactRockScore = Mathf.Round(
                artefactShapeManager.MainArtefactShape.Artefact.Score *
                artefactShapeManager.MainArtefactShape.Health * 
                artefactShapeManager.MainArtefactShape.Exposure
            );
            
            Score += artefactRockScore;
            scoreUpdated.Invoke();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            cleaningManager.cleaningStarted.RemoveListener(ResetScore);
            cleaningManager.artefactRockSucceeded.RemoveListener(UpdateScore);
            cleaningManager.cleaningEnded.RemoveListener(UpdateScore);
        }
    }
}
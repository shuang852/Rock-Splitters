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
        
        [SerializeField] private float perfectThreshold = 0.98f;
        public UnityEvent scoreUpdated = new UnityEvent();

        public float ArtefactsCleaned { get; private set; }
        public float ArtefactsPerfected { get; private set; }
        public float TotalArtefactsHealth { get; private set; }
        public float TotalArtefactsExposure { get; private set; }

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

            ArtefactsCleaned++;

            if (artefactShapeManager.MainArtefactShape.Health >= perfectThreshold)
            {
                ArtefactsPerfected++;
            };

            TotalArtefactsExposure += artefactShapeManager.MainArtefactShape.Exposure;
            TotalArtefactsHealth += artefactShapeManager.MainArtefactShape.Health;
            
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
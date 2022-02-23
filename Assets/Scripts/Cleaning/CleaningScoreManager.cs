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
        private ArtefactShape artefactShape;

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
            artefactShape = M.GetOrThrow<ArtefactShape>();

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
            if (!(artefactShape.ArtefactExposure >= requiredArtefactExposureForScoring)) return;

            // TODO: Incorporate rock difficulty.
            // TODO: Final score = Base * Health * Cleanliness * Rock Diff
            var artefactRockScore = Mathf.Round(artefactShape.Artefact.Score * artefactShape.ArtefactHealth *
                                                artefactShape.ArtefactExposure);
            ArtefactsCleaned++;

            Debug.Log(artefactShape.ArtefactHealth);
            if (artefactShape.ArtefactHealth >= perfectThreshold)
            {
                ArtefactsPerfected++;
            };

            TotalArtefactsExposure += artefactShape.ArtefactExposure;
            TotalArtefactsHealth += artefactShape.ArtefactHealth;
            
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
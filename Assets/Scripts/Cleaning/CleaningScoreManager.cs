using Managers;
using RockSystem.Artefacts;
using UnityEngine;

namespace Cleaning
{
    public class CleaningScoreManager : Manager
    {
        [Tooltip("Multiplied by the time remaining in seconds.")]
        [SerializeField] private float timeBonusMultiplier;

        private CleaningManager cleaningManager;
        private CleaningTimerManager timer;
        private ArtefactShape artefactShape;

        public float Score { get; private set; }

        protected override void Start()
        {
            base.Start();
            
            cleaningManager = M.GetOrThrow<CleaningManager>();
            timer = M.GetOrThrow<CleaningTimerManager>();
            artefactShape = M.GetOrThrow<ArtefactShape>();

            cleaningManager.cleaningEnded.AddListener(CalculateScore);
        }
        
        private void CalculateScore()
        {
            // TODO: Incorporate rock difficulty.
            // TODO: Final score = Base * Health * Cleanliness * Rock Diff + (Time + bonuses)
            Score = Mathf.Round(artefactShape.Artefact.Score * artefactShape.ArtefactHealth * artefactShape.ArtefactExposure + timer.CurrentTime * timeBonusMultiplier);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            cleaningManager.cleaningEnded.RemoveListener(CalculateScore);
        }
    }
}
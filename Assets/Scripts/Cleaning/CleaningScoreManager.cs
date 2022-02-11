using Managers;
using RockSystem.Fossils;
using UnityEngine;

namespace Cleaning
{
    public class CleaningScoreManager : Manager
    {
        [Tooltip("Multiplied by the time remaining in seconds.")]
        [SerializeField] private float timeBonusMultiplier;

        private CleaningManager cleaningManager;
        private CleaningTimerManager timer;
        private FossilShape fossilShape;

        public float Score { get; private set; }

        protected override void Start()
        {
            base.Start();
            
            cleaningManager = M.GetOrThrow<CleaningManager>();
            timer = M.GetOrThrow<CleaningTimerManager>();
            fossilShape = M.GetOrThrow<FossilShape>();

            cleaningManager.CleaningEnded.AddListener(CalculateScore);
        }
        
        private void CalculateScore()
        {
            // TODO: Incorporate rock difficulty.
            // TODO: Final score = Base * Health * Cleanliness * Rock Diff + (Time + bonuses)
            Score = Mathf.Round(fossilShape.Artefact.Score * fossilShape.FossilHealth * fossilShape.FossilExposure + timer.CurrentTime * timeBonusMultiplier);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            cleaningManager.CleaningEnded.RemoveListener(CalculateScore);
        }
    }
}
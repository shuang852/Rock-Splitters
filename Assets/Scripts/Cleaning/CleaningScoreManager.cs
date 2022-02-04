using Managers;
using UnityEngine;

namespace Cleaning
{
    public class CleaningScoreManager : Manager
    {
        [Tooltip("Multiplied by the time remaining in seconds.")]
        [SerializeField] private float timeBonusMultiplier;

        private CleaningManager cleaningManager;
        private CleaningTimerManager timer;

        public float Score { get; private set; }

        protected override void Start()
        {
            base.Start();
            
            cleaningManager = M.GetOrThrow<CleaningManager>();
            timer = M.GetOrThrow<CleaningTimerManager>();

            cleaningManager.CleaningEnded.AddListener(CalculateScore);
        }
        
        private void CalculateScore()
        {
            // TODO: Incorporate rock health and exposure. Waiting for AMG-6.
            Score = timer.CurrentTime * timeBonusMultiplier;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            cleaningManager.CleaningEnded.RemoveListener(CalculateScore);
        }
    }
}
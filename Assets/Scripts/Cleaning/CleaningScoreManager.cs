using Managers;
using UnityEngine;

namespace Cleaning
{
    public class CleaningScoreManager : Manager
    {
        [SerializeField] private CleaningTimerManager timer;
        [Tooltip("Multiplied by the time remaining in seconds.")]
        [SerializeField] private float timeBonusMultiplier;

        private CleaningManager cleaningManager;
        
        public float Score { get; private set; }

        protected override void Start()
        {
            base.Start();
            
            cleaningManager = M.GetOrThrow<CleaningManager>();

            cleaningManager.CleaningLost.AddListener(CalculateScore);
            cleaningManager.CleaningWon.AddListener(CalculateScore);
        }
        
        private void CalculateScore()
        {
            // TODO: Incorporate rock health and exposure. Waiting for AMG-6.
            Score = timer.CurrentTime * timeBonusMultiplier;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            cleaningManager.CleaningLost.RemoveListener(CalculateScore);
            cleaningManager.CleaningWon.RemoveListener(CalculateScore);
        }
    }
}
using Managers;
using UnityEngine;

namespace Cleaning
{
    public class CleaningScore : MonoBehaviour
    {
        [SerializeField] private CleaningTimerManager timer;
        [Tooltip("Multiplied by the time remaining in seconds.")]
        [SerializeField] private float timeBonusMultiplier;

        private CleaningManager cleaningManager;
        
        public float Score { get; private set; }

        private void Start()
        {
            cleaningManager = M.GetOrThrow<CleaningManager>();

            cleaningManager.CleaningLost.AddListener(CalculateScore);
            cleaningManager.CleaningWon.AddListener(CalculateScore);
        }
        
        private void CalculateScore()
        {
            // TODO: Incorporate rock health and exposure. Waiting for AMG-6.
            Score = timer.CurrentTime * timeBonusMultiplier;
        }

        private void OnDestroy()
        {
            cleaningManager.CleaningLost.RemoveListener(CalculateScore);
            cleaningManager.CleaningWon.RemoveListener(CalculateScore);
        }
    }
}
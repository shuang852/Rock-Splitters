using Managers;
using UnityEngine;

namespace Cleaning
{
    public class CleaningTimer : MonoBehaviour
    {
        [SerializeField] private float startTime;

        public float CurrentTime { get; private set; }

        private bool timerActive;

        private CleaningManager cleaningManager;

        private void Start()
        {
            cleaningManager = M.GetOrThrow<CleaningManager>();
            
            cleaningManager.CleaningStarted.AddListener(ResetAndStartTimer);
            cleaningManager.CleaningLost.AddListener(StopTimer);
            cleaningManager.CleaningWon.AddListener(StopTimer);
        }

        private void Update()
        {
            if (!timerActive) return;
            
            CurrentTime -= Time.deltaTime;

            if (!(CurrentTime <= 0)) return;
            
            CurrentTime = 0;

            timerActive = false;
            
            cleaningManager.LoseCleaning();
        }

        public void ResetTimer()
        {
            CurrentTime = startTime;
        }

        public void StartTimer()
        {
            timerActive = true;
        }

        public void StopTimer()
        {
            timerActive = false;
        }

        public void ResetAndStartTimer()
        {
            ResetTimer();
            StartTimer();
        }

        private void OnDestroy()
        {
            cleaningManager.CleaningStarted.RemoveListener(ResetAndStartTimer);
            cleaningManager.CleaningLost.RemoveListener(StopTimer);
            cleaningManager.CleaningWon.RemoveListener(StopTimer);
        }
    }
}
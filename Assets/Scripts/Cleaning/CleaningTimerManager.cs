using Managers;
using UnityEngine;
using UnityEngine.Events;

namespace Cleaning
{
    public class CleaningTimerManager : Manager
    {
        [SerializeField] private float startTime;
        [SerializeField] private float artefactRockSucceededBonus;
        [SerializeField] private float artefactRockFailedPenalty;

        public UnityEvent timeChanged = new UnityEvent();

        public float CurrentTime
        {
            get => currentTime;
            private set
            {
                currentTime = value;
                
                timeChanged.Invoke();
            }
        }

        private bool timerActive;

        private CleaningManager cleaningManager;
        private float currentTime;

        protected override void Start()
        {
            base.Start();
            
            cleaningManager = M.GetOrThrow<CleaningManager>();
            
            cleaningManager.cleaningStarted.AddListener(ResetAndStartTimer);
            cleaningManager.cleaningEnded.AddListener(StopTimer);
            cleaningManager.artefactRockSucceeded.AddListener(OnArtefactRockSucceeded);
            cleaningManager.artefactRockFailed.AddListener(OnArtefactRockFailed);
        }

        private void OnArtefactRockSucceeded()
        {
            currentTime += artefactRockSucceededBonus;
        }

        private void OnArtefactRockFailed()
        {
            currentTime -= artefactRockFailedPenalty;
        }

        protected override void Update()
        {
            base.Update();
            
            if (!timerActive) return;
            
            CurrentTime -= Time.deltaTime;

            if (!(CurrentTime <= 0)) return;
            
            CurrentTime = 0;

            timerActive = false;
            
            cleaningManager.EndCleaning();
        }

        public void ResetTimer()
        {
            CurrentTime = startTime;
        }

        public void StartTimer()
        {
            // Prevents game ending when pausing the game without having started cleaning
            // Can remove later if the cleaning phase doesn't start paused
            if (currentTime != 0)
            {
                timerActive = true;
            }
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

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            cleaningManager.cleaningStarted.RemoveListener(ResetAndStartTimer);
            cleaningManager.cleaningEnded.RemoveListener(StopTimer);
            cleaningManager.artefactRockSucceeded.RemoveListener(OnArtefactRockSucceeded);
            cleaningManager.artefactRockFailed.RemoveListener(OnArtefactRockFailed);
        }
    }
}
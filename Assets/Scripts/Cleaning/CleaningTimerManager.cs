using Managers;
using UnityEngine;
using UnityEngine.Events;

namespace Cleaning
{
    public class CleaningTimerManager : Manager
    {
        [SerializeField] private float startTime;

        public UnityEvent TimeChanged = new UnityEvent();

        public float CurrentTime
        {
            get => currentTime;
            private set
            {
                currentTime = value;
                
                TimeChanged.Invoke();
            }
        }

        private bool timerActive;

        private CleaningManager cleaningManager;
        private float currentTime;

        protected override void Start()
        {
            base.Start();
            
            cleaningManager = M.GetOrThrow<CleaningManager>();
            
            cleaningManager.CleaningStarted.AddListener(ResetAndStartTimer);
            cleaningManager.CleaningLost.AddListener(StopTimer);
            cleaningManager.CleaningWon.AddListener(StopTimer);
        }

        protected override void Update()
        {
            base.Update();
            
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

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            cleaningManager.CleaningStarted.RemoveListener(ResetAndStartTimer);
            cleaningManager.CleaningLost.RemoveListener(StopTimer);
            cleaningManager.CleaningWon.RemoveListener(StopTimer);
        }
    }
}
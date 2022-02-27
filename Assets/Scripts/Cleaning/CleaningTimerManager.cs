using Managers;
using RockSystem.Artefacts;
using UnityEngine;
using UnityEngine.Events;

namespace Cleaning
{
    public class CleaningTimerManager : Manager
    {
        [SerializeField] private float startTime;
        [Tooltip("The amount of time added to the clock based on artefact health.")]
        [SerializeField] private AnimationCurve artefactRockCompletionBonusCurve;

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

        public float TotalTime { get; private set; }

        private CleaningManager cleaningManager;
        private ArtefactShapeManager artefactShapeManager;

        private bool timerActive;
        private float currentTime;

        protected override void Start()
        {
            base.Start();
            
            cleaningManager = M.GetOrThrow<CleaningManager>();
            artefactShapeManager = M.GetOrThrow<ArtefactShapeManager>();

            TotalTime = startTime;
            
            cleaningManager.cleaningStarted.AddListener(ResetAndStartTimer);
            cleaningManager.cleaningEnded.AddListener(StopTimer);
            cleaningManager.artefactRockCompleted.AddListener(OnArtefactRockCompleted);
            cleaningManager.cleaningPaused.AddListener(StopTimer);
            cleaningManager.cleaningResumed.AddListener(StartTimer);
        }

        private void OnArtefactRockCompleted()
        {
            var bonusTime = artefactRockCompletionBonusCurve.Evaluate(artefactShapeManager.Health);
            CurrentTime += bonusTime;
            TotalTime += bonusTime;
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
            if (CurrentTime != 0)
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
            cleaningManager.artefactRockCompleted.RemoveListener(OnArtefactRockCompleted);
            cleaningManager.cleaningPaused.RemoveListener(StopTimer);
            cleaningManager.cleaningResumed.RemoveListener(StartTimer);
        }
    }
}
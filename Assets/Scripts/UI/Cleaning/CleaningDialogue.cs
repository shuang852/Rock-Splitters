using System;
using Cleaning;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Managers;
using UI.Cleaning_Results;
using UI.Core;
using UnityEngine;

namespace UI.Cleaning
{
    public class CleaningDialogue : Dialogue
    {
        [SerializeField] private GameObject cleaningArtefactResultsDialoguePrefab;
        [SerializeField] private GameObject cleaningCompletedResultsDialoguePrefab;
        [SerializeField] private GameObject cleaningCountdownDialoguePrefab;
        [SerializeField] private GameObject readyPromptDialoguePrefab;
        [SerializeField] private BrushInput brushInput;
        [SerializeField] private Canvas canvas;
        
        [Header("Transition settings")]
        [SerializeField] private GameObject cleaningArea;
        [SerializeField] private float delayAfterArtefactCleaned = 1.5f;
        [SerializeField] private float rockMoveDuration = 2f;
        [SerializeField] private float rockMoveWorldPosX = 15;
        

        private CleaningManager cleaningManager;
        private SelectToolButton activeToolButton;

        private bool readyPromptShown;

        private void Start()
        {
            cleaningManager = M.GetOrThrow<CleaningManager>();

            cleaningManager.cleaningStarted.AddListener(OnCleaningStarted);
            cleaningManager.cleaningEnded.AddListener(ShowFinalResults);
            cleaningManager.artefactStatsCompleted.AddListener(ShowArtefactResults);

            canvas.worldCamera = Camera.main;

            brushInput.enabled = false;
            
            ShowReadyPrompt();
        }

        private void OnCleaningStarted()
        {
            brushInput.enabled = true;
            
            ShowCountdown();
        }

        private async void ShowArtefactResults()
        {
            brushInput.enabled = false;

            await UniTask.Delay(TimeSpan.FromSeconds(delayAfterArtefactCleaned));
            
            var results = Instantiate(cleaningArtefactResultsDialoguePrefab, transform.parent)
                .GetComponent<CleaningFossilResultsDialogue>();
            
            await UniTask.Delay(TimeSpan.FromSeconds(results.TotalDuration));
            
            await cleaningArea.transform.DOMoveX(rockMoveWorldPosX, rockMoveDuration).SetEase(Ease.OutQuad)
                .AsyncWaitForCompletion();

            cleaningArea.transform.position = new Vector3(-rockMoveWorldPosX, 0, 0);
            cleaningManager.NextArtefactRock();

            await cleaningArea.transform.DOMoveX(0, rockMoveDuration).SetEase(Ease.InQuad)
                .AsyncWaitForCompletion();
            manager.Pop();
            cleaningManager.ResumeCleaning();
            cleaningManager.nextArtefactRockStarted.Invoke();
            brushInput.enabled = true;
        }
        
        private void ShowFinalResults()
        {
            brushInput.enabled = false;
            
            Instantiate(cleaningCompletedResultsDialoguePrefab, transform.parent);
        }

        private void ShowCountdown()
        {
            Instantiate(cleaningCountdownDialoguePrefab, transform.parent);
        }

        private void ShowReadyPrompt()
        {
            readyPromptShown = true;

            Instantiate(readyPromptDialoguePrefab, transform.parent);
        }

        protected override void OnClose() { }

        // TODO: This is gross
        protected override void OnPromote()
        {
            if (!readyPromptShown) return;
            
            cleaningManager.StartCleaning();
            
            readyPromptShown = false;
        }

        protected override void OnDemote() { }

        private void OnDestroy()
        {
            cleaningManager.cleaningStarted.RemoveListener(OnCleaningStarted);
            cleaningManager.cleaningEnded.RemoveListener(ShowFinalResults);
            cleaningManager.artefactStatsCompleted.RemoveListener(ShowArtefactResults);
        }
        
        public void DeselectToolButton(SelectToolButton selectToolButton)
        {
            if (activeToolButton)
                activeToolButton.DeselectButton();
            
            activeToolButton = selectToolButton;
        }
    }
}
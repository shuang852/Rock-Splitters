using Cleaning;
using Managers;
using UI.Core;
using UnityEngine;

namespace UI.Cleaning
{
    public class CleaningDialogue : Dialogue
    {
        [SerializeField] private GameObject cleaningResultsDialoguePrefab;
        [SerializeField] private GameObject cleaningCountdownDialoguePrefab;
        [SerializeField] private GameObject readyPromptDialoguePrefab;
        [SerializeField] private BrushInput brushInput;

        private CleaningManager cleaningManager;
        private SelectToolButton activeToolButton;

        private bool readyPromptShown;

        private void Start()
        {
            cleaningManager = M.GetOrThrow<CleaningManager>();

            cleaningManager.cleaningStarted.AddListener(OnCleaningStarted);
            cleaningManager.cleaningEnded.AddListener(ShowResults);
            cleaningManager.nextArtefactRock.AddListener(ShowCountdown);

            brushInput.enabled = false;
            
            ShowReadyPrompt();
        }

        private void OnCleaningStarted()
        {
            brushInput.enabled = true;
        }

        private void ShowResults()
        {
            brushInput.enabled = false;
            
            Instantiate(cleaningResultsDialoguePrefab, transform.parent);
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
            cleaningManager.cleaningEnded.RemoveListener(ShowResults);
            cleaningManager.nextArtefactRock.RemoveListener(ShowCountdown);
        }
        
        public void DeselectToolButton(SelectToolButton selectToolButton)
        {
            if (activeToolButton)
                activeToolButton.DeselectButton();
            
            activeToolButton = selectToolButton;
        }
    }
}
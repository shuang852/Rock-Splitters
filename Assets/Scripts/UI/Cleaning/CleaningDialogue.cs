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
        [SerializeField] private BrushInput brushInput;

        private CleaningManager cleaningManager;
        private SelectToolButton activeToolButton;

        private void Start()
        {
            cleaningManager = M.GetOrThrow<CleaningManager>();

            cleaningManager.cleaningStarted.AddListener(OnCleaningStarted);
            cleaningManager.cleaningEnded.AddListener(ShowResults);
            cleaningManager.nextArtefactRock.AddListener(OnNextArtefactRock);

            brushInput.enabled = false;
        }

        private void OnNextArtefactRock()
        {
            Instantiate(cleaningCountdownDialoguePrefab, transform.parent);
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

        protected override void OnClose() { }

        protected override void OnPromote() { }

        protected override void OnDemote() { }

        private void OnDestroy()
        {
            cleaningManager.cleaningStarted.RemoveListener(OnCleaningStarted);
            cleaningManager.cleaningEnded.RemoveListener(ShowResults);
            cleaningManager.nextArtefactRock.RemoveListener(OnNextArtefactRock);
        }
        
        public void DeselectToolButton(SelectToolButton selectToolButton)
        {
            if (activeToolButton)
                activeToolButton.DeselectButton();
            
            activeToolButton = selectToolButton;
        }
    }
}
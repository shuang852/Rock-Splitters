using Cleaning;
using Managers;
using UI.Core;
using UnityEngine;

namespace UI.Cleaning
{
    public class CleaningDialogue : Dialogue
    {
        [SerializeField] private GameObject cleaningResultsDialoguePrefab;
        [SerializeField] private BrushInput brushInput;

        private CleaningManager cleaningManager;
        private SelectToolButton activeToolButton;

        private void Start()
        {
            cleaningManager = M.GetOrThrow<CleaningManager>();

            cleaningManager.CleaningStarted.AddListener(OnCleaningStarted);
            cleaningManager.CleaningEnded.AddListener(ShowResults);

            brushInput.enabled = false;
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
            cleaningManager.CleaningStarted.RemoveListener(OnCleaningStarted);
            cleaningManager.CleaningEnded.RemoveListener(ShowResults);
        }
        
        public void DeselectToolButton(SelectToolButton selectToolButton)
        {
            if (activeToolButton)
                activeToolButton.DeselectButton();
            
            activeToolButton = selectToolButton;
        }
    }
}
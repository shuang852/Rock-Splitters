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

        private void Start()
        {
            cleaningManager = M.GetOrThrow<CleaningManager>();

            cleaningManager.CleaningStarted.AddListener(OnCleaningStarted);
            cleaningManager.CleaningWon.AddListener(ShowResults);
            cleaningManager.CleaningLost.AddListener(ShowResults);

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
            cleaningManager.CleaningWon.RemoveListener(ShowResults);
            cleaningManager.CleaningLost.RemoveListener(ShowResults);
        }
    }
}
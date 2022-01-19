using Cleaning;
using Managers;
using UI.Core;
using UnityEngine;

namespace UI.Cleaning
{
    public class CleaningDialogue : Dialogue
    {
        [SerializeField] private GameObject cleaningResultsDialoguePrefab;

        private CleaningManager cleaningManager;

        private void Start()
        {
            cleaningManager = M.GetOrThrow<CleaningManager>();

            cleaningManager.CleaningWon.AddListener(ShowResults);
            cleaningManager.CleaningLost.AddListener(ShowResults);
        }

        private void ShowResults()
        {
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
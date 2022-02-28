using System.Globalization;
using Cleaning;
using Managers;
using RockSystem.Artefacts;
using TMPro;
using UI.Core;
using UnityEngine;

namespace UI.Cleaning_Results
{
    public class ScoreFossilText : DialogueComponent<CleaningFossilResultsDialogue>
    {
        [SerializeField] private TextMeshProUGUI motivationText;
        [SerializeField] private TextMeshProUGUI totalScoreText;
        [SerializeField] private TextMeshProUGUI artefactScoreText;
        [SerializeField] private TextMeshProUGUI artefactHealthText;
        [SerializeField] private TextMeshProUGUI bonusText;
        [SerializeField] private TextMeshProUGUI timeTakenText;

        private CleaningManager cleaningManager;
        private CleaningTimerManager timerManager;
        private CleaningScoreManager scoreManager;
        private ArtefactShape artefactShape;

        protected override void OnComponentStart()
        {
            base.OnComponentStart();

            cleaningManager = M.GetOrThrow<CleaningManager>();
            timerManager = M.GetOrThrow<CleaningTimerManager>();
            scoreManager = M.GetOrThrow<CleaningScoreManager>();
            artefactShape = M.GetOrThrow<ArtefactShape>();

            //baseArtefactScoreText.text = "Not implemented";
            artefactHealthText.text =
                Mathf.Round(artefactShape.ArtefactHealth * 100).ToString(CultureInfo.InvariantCulture) + "%";
            //timeRemainingText.text = timerManager.CurrentTime.ToString(CultureInfo.InvariantCulture);
            totalScoreText.text = scoreManager.Score.ToString(CultureInfo.InvariantCulture);
        }

        protected override void Subscribe() { }

        protected override void Unsubscribe() { }

        // TODO
        // private string WinStateToString()
        // {
        //     return "Times Up!";
        //
        //     // switch (cleaningManager.CurrentCleaningState)
        //     // {
        //     //     case CleaningManager.CleaningState.Lost:
        //     //         return "Better luck next time...";
        //     //     case CleaningManager.CleaningState.Won:
        //     //         return "Success!";
        //     //     default:
        //     //         Debug.Log($"Invalid {nameof(CleaningManager.CleaningState)} {cleaningManager.CurrentCleaningState}.");
        //     //         return "Invalid State";
        //     // }
        // }
    }
}
using System.Globalization;
using Cleaning;
using Managers;
using RockSystem.Artefacts;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Cleaning_Results
{
    public class ScoreFossilText : DialogueComponent<CleaningFossilResultsDialogue>
    {
        [SerializeField] private Text winStateText;
        [SerializeField] private Text baseArtefactScoreText;
        [SerializeField] private Text artefactHealthText;
        [SerializeField] private Text artefactExposureText;
        [SerializeField] private Text timeRemainingText;
        [SerializeField] private Text totalScoreText;

        private CleaningManager cleaningManager;
        private CleaningTimerManager timerManager;
        private CleaningScoreManager scoreManager;
        private ArtefactShapeManager artefactShapeManager;

        protected override void OnComponentStart()
        {
            base.OnComponentStart();

            cleaningManager = M.GetOrThrow<CleaningManager>();
            timerManager = M.GetOrThrow<CleaningTimerManager>();
            scoreManager = M.GetOrThrow<CleaningScoreManager>();
            artefactShapeManager = M.GetOrThrow<ArtefactShapeManager>();

            winStateText.text = WinStateToString();
            baseArtefactScoreText.text = "Not implemented";
            artefactHealthText.text =
                Mathf.Round(artefactShapeManager.Health * 100).ToString(CultureInfo.InvariantCulture) + "%";
            artefactExposureText.text =
                Mathf.Round(artefactShapeManager.Exposure * 100).ToString(CultureInfo.InvariantCulture) + "%";
            timeRemainingText.text = timerManager.CurrentTime.ToString(CultureInfo.InvariantCulture);
            totalScoreText.text = scoreManager.Score.ToString(CultureInfo.InvariantCulture);
        }

        protected override void Subscribe() { }

        protected override void Unsubscribe() { }

        // TODO
        private string WinStateToString()
        {
            return "Times Up!";

            // switch (cleaningManager.CurrentCleaningState)
            // {
            //     case CleaningManager.CleaningState.Lost:
            //         return "Better luck next time...";
            //     case CleaningManager.CleaningState.Won:
            //         return "Success!";
            //     default:
            //         Debug.Log($"Invalid {nameof(CleaningManager.CleaningState)} {cleaningManager.CurrentCleaningState}.");
            //         return "Invalid State";
            // }
        }
    }
}
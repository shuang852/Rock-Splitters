using System.Globalization;
using Cleaning;
using Managers;
using RockSystem.Artefacts;
using UI.Core;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Cleaning_Results
{
    public class ScoreText : DialogueComponent<CleaningResultsDialogue>
    {
        [SerializeField] private Text winStateText;
        [FormerlySerializedAs("baseFossilScoreText")] [SerializeField] private Text baseArtefactScoreText;
        [FormerlySerializedAs("fossilHealthText")] [SerializeField] private Text artefactHealthText;
        [FormerlySerializedAs("fossilExposureText")] [SerializeField] private Text artefactExposureText;
        [SerializeField] private Text timeRemainingText;
        [SerializeField] private Text totalScoreText;

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

            winStateText.text = WinStateToString();
            baseArtefactScoreText.text = "Not implemented";
            artefactHealthText.text =
                Mathf.Round(artefactShape.ArtefactHealth * 100).ToString(CultureInfo.InvariantCulture) + "%";
            artefactExposureText.text =
                Mathf.Round(artefactShape.ArtefactExposure * 100).ToString(CultureInfo.InvariantCulture) + "%";
            timeRemainingText.text = timerManager.CurrentTime.ToString(CultureInfo.InvariantCulture);
            totalScoreText.text = scoreManager.Score.ToString(CultureInfo.InvariantCulture);
        }

        protected override void Subscribe() { }

        protected override void Unsubscribe() { }

        private string WinStateToString()
        {
            switch (cleaningManager.CurrentCleaningState)
            {
                case CleaningManager.CleaningState.Lost:
                    return "Better luck next time...";
                case CleaningManager.CleaningState.Won:
                    return "Success!";
                default:
                    Debug.Log($"Invalid {nameof(CleaningManager.CleaningState)} {cleaningManager.CurrentCleaningState}.");
                    return "Invalid State";
            }
        }
    }
}
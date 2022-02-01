using System.Globalization;
using Cleaning;
using Managers;
using RockSystem.Fossils;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Cleaning_Results
{
    public class ScoreText : DialogueComponent<CleaningResultsDialogue>
    {
        [SerializeField] private Text winStateText;
        [SerializeField] private Text baseFossilScoreText;
        [SerializeField] private Text fossilHealthText;
        [SerializeField] private Text fossilExposureText;
        [SerializeField] private Text timeRemainingText;
        [SerializeField] private Text totalScoreText;

        private CleaningManager cleaningManager;
        private CleaningTimerManager timerManager;
        private CleaningScoreManager scoreManager;
        private FossilShape fossilShape;

        protected override void OnComponentStart()
        {
            base.OnComponentStart();

            cleaningManager = M.GetOrThrow<CleaningManager>();
            timerManager = M.GetOrThrow<CleaningTimerManager>();
            scoreManager = M.GetOrThrow<CleaningScoreManager>();
            fossilShape = M.GetOrThrow<FossilShape>();

            winStateText.text = WinStateToString();
            baseFossilScoreText.text = "Not implemented";
            fossilHealthText.text =
                Mathf.Round(fossilShape.FossilHealth() * 100).ToString(CultureInfo.InvariantCulture) + "%";
            fossilExposureText.text =
                Mathf.Round(fossilShape.FossilExposure() * 100).ToString(CultureInfo.InvariantCulture) + "%";
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
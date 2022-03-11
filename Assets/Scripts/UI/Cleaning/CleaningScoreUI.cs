using System.Globalization;
using Cleaning;
using Managers;
using TMPro;
using UI.Core;

namespace UI.Cleaning
{
    public class CleaningScoreUI : DialogueComponent<CleaningDialogue>
    {
        private CleaningScoreManager manager;
        private TextMeshProUGUI text;

        protected override void OnComponentAwake()
        {
            TryGetComponent(out text);
        }

        protected override void OnComponentStart()
        {
            base.OnComponentStart();
            manager = M.GetOrThrow<CleaningScoreManager>();
            
            manager.scoreUpdated.AddListener(UpdateScore);
        }

        protected override void Subscribe() { }

        protected override void Unsubscribe() { }
        

        private void UpdateScore()
        {
            text.text = "Score: " + manager.TotalScore.ToString(CultureInfo.InvariantCulture);
        }
    }
}
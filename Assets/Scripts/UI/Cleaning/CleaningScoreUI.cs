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
        }

        protected override void Subscribe() { }

        protected override void Unsubscribe() { }

        // TODO: Turn this to event based
        private void Update()
        {
            text.text = manager.Score.ToString(CultureInfo.InvariantCulture);
        }
    }
}
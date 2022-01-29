using System.Globalization;
using Managers;
using RockSystem.Fossils;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Cleaning
{
    public class FossilHealth : DialogueComponent<CleaningDialogue>
    {
        [SerializeField] private Text text;

        private FossilShape fossilShape;

        protected override void OnComponentStart()
        {
            base.OnComponentStart();

            fossilShape = M.GetOrThrow<FossilShape>();
            
            fossilShape.fossilDamaged.AddListener(UpdateText);
            
            // BUG: Race condition with FossilShape leads to NaN being displayed.
            UpdateText();
        }

        private void UpdateText()
        {
            text.text = Mathf.Round(fossilShape.FossilHealth() * 100).ToString(CultureInfo.InvariantCulture);
        }

        protected override void Subscribe() { }

        protected override void Unsubscribe() { }
    }
}
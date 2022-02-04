using Managers;
using RockSystem.Fossils;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Cleaning
{
    public class FossilHealth : DialogueComponent<CleaningDialogue>
    {
        [SerializeField] private Image image;

        private FossilShape fossilShape;

        protected override void OnComponentStart()
        {
            base.OnComponentStart();

            fossilShape = M.GetOrThrow<FossilShape>();
            
            fossilShape.fossilDamaged.AddListener(UpdateHealth);
            
            // BUG: Race condition with FossilShape leads to NaN being displayed.
            UpdateHealth();
        }

        private void UpdateHealth()
        {
            image.fillAmount = 1 - fossilShape.FossilHealth;
        }

        protected override void Subscribe() { }

        protected override void Unsubscribe() { }
    }
}
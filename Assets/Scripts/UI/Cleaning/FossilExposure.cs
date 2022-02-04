using Managers;
using RockSystem.Fossils;
using ToolSystem;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Cleaning
{
    public class FossilExposure : DialogueComponent<CleaningDialogue>
    {
        [SerializeField] private Image image;

        private FossilShape fossilShape;

        protected override void OnComponentStart()
        {
            base.OnComponentStart();

            fossilShape = M.GetOrThrow<FossilShape>();
            
            fossilShape.fossilExposed.AddListener(UpdateExposure);
            fossilShape.fossilDamaged.AddListener(UpdateExposure);
            
            // BUG: Race condition with FossilShape leads to NaN being displayed.
            UpdateExposure();
        }

        private void UpdateExposure()
        {
            image.fillAmount = fossilShape.FossilExposure - (1 - fossilShape.FossilHealth);
        }

        protected override void Subscribe() { }

        protected override void Unsubscribe() { }
    }
}
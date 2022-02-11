using Managers;
using RockSystem.Artefacts;
using ToolSystem;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Cleaning
{
    public class ArtefactExposure : DialogueComponent<CleaningDialogue>
    {
        [SerializeField] private Image image;

        private ArtefactShape artefactShape;

        protected override void OnComponentStart()
        {
            base.OnComponentStart();

            artefactShape = M.GetOrThrow<ArtefactShape>();
            
            artefactShape.artefactExposed.AddListener(UpdateExposure);
            artefactShape.artefactDamaged.AddListener(UpdateExposure);
            
            // BUG: Race condition with ArtefactShape leads to NaN being displayed.
            UpdateExposure();
        }

        private void UpdateExposure()
        {
            image.fillAmount = artefactShape.ArtefactExposure - (1 - artefactShape.ArtefactHealth);
        }

        protected override void Subscribe() { }

        protected override void Unsubscribe() { }
    }
}
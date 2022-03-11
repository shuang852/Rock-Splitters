using Managers;
using RockSystem.Artefacts;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Cleaning
{
    public class ArtefactHealth : DialogueComponent<CleaningDialogue>
    {
        [SerializeField] private Image image;

        private ArtefactShapeManager artefactShapeManager;

        protected override void OnComponentStart()
        {
            base.OnComponentStart();

            artefactShapeManager = M.GetOrThrow<ArtefactShapeManager>();
            
            artefactShapeManager.artefactDamaged.AddListener(UpdateHealth);
            
            // BUG: Race condition with ArtefactShape leads to NaN being displayed.
            UpdateHealth();
        }

        private void UpdateHealth()
        {
            image.fillAmount = 1 - artefactShapeManager.Health;
        }

        protected override void Subscribe() { }

        protected override void Unsubscribe() { }

        private void OnDestroy()
        {
            artefactShapeManager.artefactDamaged.RemoveListener(UpdateHealth);
        }
    }
}
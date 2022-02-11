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

        private ArtefactShape artefactShape;

        protected override void OnComponentStart()
        {
            base.OnComponentStart();

            artefactShape = M.GetOrThrow<ArtefactShape>();
            
            artefactShape.artefactDamaged.AddListener(UpdateHealth);
            
            // BUG: Race condition with ArtefactShape leads to NaN being displayed.
            UpdateHealth();
        }

        private void UpdateHealth()
        {
            image.fillAmount = 1 - artefactShape.ArtefactHealth;
        }

        protected override void Subscribe() { }

        protected override void Unsubscribe() { }

        private void OnDestroy()
        {
            artefactShape.artefactDamaged.RemoveListener(UpdateHealth);
        }
    }
}
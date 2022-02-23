using Managers;
using RockSystem.Artefacts;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Cleaning
{
    public class ArtefactExposure : DialogueComponent<CleaningDialogue>
    {
        [SerializeField] private Image image;

        private ArtefactShapeManager artefactShapeManager;

        protected override void OnComponentStart()
        {
            base.OnComponentStart();

            artefactShapeManager = M.GetOrThrow<ArtefactShapeManager>();
            
            artefactShapeManager.artefactExposed.AddListener(UpdateExposure);
            artefactShapeManager.artefactDamaged.AddListener(OnArtefactDamaged);
            
            // BUG: Race condition with ArtefactShape leads to NaN being displayed.
            UpdateExposure();
        }

        private void OnArtefactDamaged(ArtefactShape artefactShape, Vector2Int position)
        {
            UpdateExposure();
        }

        private void UpdateExposure()
        {
            image.fillAmount = artefactShapeManager.Exposure - (1 - artefactShapeManager.Health);
        }

        protected override void Subscribe() { }

        protected override void Unsubscribe() { }

        private void OnDestroy()
        {
            artefactShapeManager.artefactExposed.RemoveListener(UpdateExposure);
            artefactShapeManager.artefactDamaged.RemoveListener(OnArtefactDamaged);
        }
    }
}
using Cysharp.Threading.Tasks;
using Managers;
using Stored;
using UI.Core;
using UI.Generic;
using UnityEngine;

namespace UI.Exhibition
{
    public class ExhibitionDialogue : Dialogue
    {
        [SerializeField] private ScrollviewSnapping scroll;
        [SerializeField] private GameObject setPrefab;
        
        private ArtefactManager artefactManager;
        protected override void OnClose() {}

        protected override void OnPromote() {}

        protected override void OnDemote() {}

        private async void Start()
        {
            artefactManager = M.GetOrThrow<ArtefactManager>();
            foreach (Transform child in scroll.Panel.transform)
            {
               Destroy(child.gameObject);
            }

            await UniTask.Yield();
            
            foreach (var artefactSet in artefactManager.artefactSetDatabase.OrderedItems)
            {
                var go = Instantiate(setPrefab, scroll.Panel.transform);
                go.GetComponent<ArtefactSetUI>().Setup(artefactSet);
            }
            await UniTask.Yield();
            scroll.CalculateChildren();
        }
    }
}
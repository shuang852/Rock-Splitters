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
        
        private AntiquityManager antiquityManager;
        protected override void OnClose() {}

        protected override void OnPromote() {}

        protected override void OnDemote() {}

        private async void Start()
        {
            antiquityManager = M.GetOrThrow<AntiquityManager>();
            foreach (Transform child in scroll.Panel.transform)
            {
               Destroy(child.gameObject);
            }

            await UniTask.Yield();
            
            foreach (var antiquitySet in antiquityManager.antiquitySetDatabase.OrderedItems)
            {
                var go = Instantiate(setPrefab, scroll.Panel.transform);
                go.GetComponent<AntiquitySetUI>().Setup(antiquitySet);
            }
            scroll.CalculateChildren();
        }
    }
}
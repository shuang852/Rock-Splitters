using Managers;
using Stored;
using TMPro;
using UI.AntiquitySets;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Exhibition
{
    public class AntiquitySetUI : MonoBehaviour
    {
        public AntiquitySet antiquitySet;
        [SerializeField] private GameObject collection;
        [SerializeField] private Image setImage;
        [SerializeField] private TextMeshProUGUI setNameText;
        [SerializeField] private GameObject setItemPrefab;

        //private AntiquityManager antiquityManager;
        private Inventory inventory;

        public void Setup(AntiquitySet set)
        {
            inventory = M.GetOrThrow<AntiquityManager>().Inventory;

            antiquitySet = set;
            
            setImage.sprite = antiquitySet.Sprite;
            setNameText.text = antiquitySet.SetName;

            // If we run into performance issues in the future for this, we can store a completed variable in AntiquitySet
            foreach (var antiquity in antiquitySet.SetItems)
            {
                var obj = Instantiate(setItemPrefab, collection.transform);
                var hasItem = inventory.Contains(antiquity);
                obj.GetComponent<AntiquitySetItemUI>().SetupSetItem(antiquity, hasItem);
            }
        }
    }
}

using Managers;
using Stored;
using TMPro;
using UI.AntiquitySets;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Exhibition
{
    public class AntiquitySetUI : DialogueComponent<ExhibitionDialogue>
    {
        public AntiquitySet antiquitySet;
        
        [Header("Left section")]
        [SerializeField] private Image setImage;
        [SerializeField] private TextMeshProUGUI setNameText;
        [SerializeField] private TextMeshProUGUI setInfoNameText;
        [SerializeField] private TextMeshProUGUI setDescriptionText;
        [SerializeField] private TextMeshProUGUI setStatsText;
        
        [Header("Right Section")]
        [SerializeField] private GameObject collection;
        [SerializeField] private GameObject setItemPrefab;

        private Inventory inventory;
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        public void Setup(AntiquitySet set)
        {
            inventory = M.GetOrThrow<AntiquityManager>().Inventory;

            antiquitySet = set;
            
            setImage.sprite = antiquitySet.Sprite;
            setNameText.text = antiquitySet.SetName;
            setInfoNameText.text = antiquitySet.SetName;
            setDescriptionText.text = antiquitySet.Description;
            setStatsText.text = $"Income/Capacity: {antiquitySet.CurrentSetIncome} / {antiquitySet.CurrentSetCapacity}";

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
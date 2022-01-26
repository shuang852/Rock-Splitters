using System;
using Managers;
using Stored;
using TMPro;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AntiquitySets
{
    public class AntiquitySetDialogue : Dialogue
    {
        [SerializeField] private AntiquitySet antiquitySet;
        [SerializeField] private GameObject collection;
        [SerializeField] private Image setImage;
        [SerializeField] private TextMeshProUGUI setNameText;
        [SerializeField] private GameObject setItemPrefab;

        private AntiquityManager antiquityManager;
        private Inventory inventory;
        
        protected override void OnClose()
        {
            
        }

        protected override void OnPromote()
        {
            
        }

        protected override void OnDemote()
        {
            
        }

        private void Start()
        {
            antiquityManager = M.GetOrThrow<AntiquityManager>();
            inventory = antiquityManager.Inventory;
            
            setImage.sprite = antiquitySet.Sprite;
            setNameText.text = antiquitySet.SetName;

            // If we run into performance issues in the future for this, we can store a completed variable that skips has
            // item.
            foreach (var antiquity in antiquitySet.SetItems)
            {
                var obj = Instantiate(setItemPrefab, collection.transform);
                var hasItem = inventory.Contains(antiquity);
                obj.GetComponent<AntiquitySetItemUI>().SetupSetItem(antiquity, hasItem);
            }

            // TODO: Move this logic to when you pick up an antiquity
            // Moved to antiquity set
            // Finds how much of a set you have and add it's total 
            // int unlockedPercentage = count / antiquitySet.SetItems.Length;
            // var bonus = unlockedPercentage % 1 * antiquitySet.SetBonus;
            // antiquityManager.AddSetStats(
            //     unlockedPercentage * bonus * antiquitySet.SetIncomeRate, unlockedPercentage * bonus * antiquitySet.SetCapacity);
        }
    }
}

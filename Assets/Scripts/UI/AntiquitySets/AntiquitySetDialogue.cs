using System;
using Fossils;
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
            inventory = antiquityManager.AntiquityInventory;
            
            setImage.sprite = antiquitySet.Sprite;
            setNameText.text = antiquitySet.SetName;
            foreach (var antiquity in antiquitySet.SetItems)
            {
                var obj = Instantiate(setItemPrefab, collection.transform);
                //obj.GetComponent<FossilSetItem>().SetupSetItem(antiquity, inventory.get);
            }
        }
    }
}

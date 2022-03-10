using Managers;
using Stored;
using TMPro;
using UI.Core;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Exhibition
{
    public class ArtefactSetUI : DialogueComponent<ExhibitionDialogue>
    {
        [FormerlySerializedAs("antiquitySet")] public ArtefactSet artefactSet;
        
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
        
        public void Setup(ArtefactSet set)
        {
            inventory = M.GetOrThrow<ArtefactManager>().Inventory;

            artefactSet = set;
            
            setImage.sprite = artefactSet.Sprite;
            setNameText.text = artefactSet.SetName;
            setInfoNameText.text = artefactSet.SetName;
            setDescriptionText.text = artefactSet.Description;
            //setStatsText.text = $"Income/Capacity: {artefactSet.CurrentSetIncome} / {artefactSet.CurrentSetCapacity}";

            // If we run into performance issues in the future for this, we can store a completed variable in ArtefactSet
            foreach (var artefact in artefactSet.SetItems)
            {
                var obj = Instantiate(setItemPrefab, collection.transform);
                var hasItem = inventory.Contains(artefact);
                var collected = 0;
                if (hasItem) collected = inventory.GetNumberOfItem(artefact);
                obj.GetComponent<ArtefactSetItemUI>().SetupSetItem(artefact, hasItem, collected);
            }
        }
    }
}
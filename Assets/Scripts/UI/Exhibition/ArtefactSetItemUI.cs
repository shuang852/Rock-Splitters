using Stored;
using TMPro;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Exhibition
{
    [RequireComponent(typeof(Button))]
    public class ArtefactSetItemUI : DialogueComponent<Dialogue>
    {

        // private Button button;
        [SerializeField] private Color lockedColour;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI numText;

        protected override void OnComponentAwake() { }

        protected override void Subscribe() { }
    
        protected override void Unsubscribe() { }

        public void SetupSetItem(Artefact setItem, bool hasItem, int num = 0)
        {
            //Debug.Log(setItem);
            icon.sprite = setItem.Sprite;
            if (!hasItem)
            {
                icon.color = lockedColour;
            }

            numText.text = num.ToString();
        }
        private void OnValidate()
        {
            if (!icon) icon = GetComponentInChildren<Image>();
            if (!numText) numText = GetComponentInChildren<TextMeshProUGUI>();
        }
    }
}
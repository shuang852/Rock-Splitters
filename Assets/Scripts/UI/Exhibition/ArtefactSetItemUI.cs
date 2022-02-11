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
        [SerializeField] private TextMeshProUGUI score;

        protected override void OnComponentAwake() { }

        protected override void Subscribe() { }
    
        protected override void Unsubscribe() { }

        public void SetupSetItem(Artefact setItem, bool hasItem)
        {
            //Debug.Log(setItem);
            icon.sprite = setItem.Sprite;
            if (!hasItem)
            {
                icon.color = lockedColour;
            }
            // TODO: Uncomment when score is added
            //icon.sprite = setItem.score;
        }
        private void OnValidate()
        {
            if (!icon) icon = GetComponentInChildren<Image>();
            if (!score) score = GetComponentInChildren<TextMeshProUGUI>();
        }
    }
}
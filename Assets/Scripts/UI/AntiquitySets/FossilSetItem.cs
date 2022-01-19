using Stored;
using TMPro;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AntiquitySets
{
    [RequireComponent(typeof(Button))]
    public class FossilSetItem : DialogueComponent<Dialogue>
    {

        // private Button button;
        [SerializeField] private Color lockedColour;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI score;

        protected override void OnComponentAwake()
        {
            // TryGetComponent(out button);
            // button.onClick.AddListener(OnSubmit);
        }
    
        protected override void OnComponentStart()
        {
            base.OnComponentStart();
             
        }
    
        protected override void Subscribe() { }
    
        protected override void Unsubscribe() { }
    
        // private void OnSubmit()
        // {
        //             
        // }

        public void SetupSetItem(Antiquity setItem, bool hasItem)
        {
            icon.sprite = setItem.Sprite;
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
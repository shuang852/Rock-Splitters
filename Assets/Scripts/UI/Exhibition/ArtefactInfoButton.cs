using DG.Tweening;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Exhibition
{
    [RequireComponent(typeof(Button))]
    public class ArtefactInfoButton : DialogueComponent<ExhibitionDialogue>
    {
        [SerializeField] private GameObject flipUp;
        [SerializeField] private GameObject flipDown;
        
        private Button dialogueButton;
        private int index = 1;

        protected override void OnComponentAwake()
        {
            TryGetComponent(out dialogueButton);
            dialogueButton.onClick.AddListener(OnSubmit);
        }

        protected override void Subscribe() { }

        private void OnSubmit()
        {
            flipUp.transform.DORotateQuaternion(
                Quaternion.Euler(new Vector3(0 + 180 * (index % 2), 0, 0)), 1);
            flipDown.transform.DORotateQuaternion(
                Quaternion.Euler(new Vector3(180 + 180 * (index % 2),0,0)),1);
            flipUp.gameObject.SetActive(index % 2 == 0);
            flipDown.gameObject.SetActive(index % 2 == 1);
            index++;
        }

        protected override void Unsubscribe() { }
    }
}
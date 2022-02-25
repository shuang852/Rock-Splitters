using System;
using Cysharp.Threading.Tasks;
using Managers;
using TMPro;
using UnityEngine;

namespace UI.Cleaning
{
    public enum TextSize
    {
        Small =6,
        Medium = 8,
        Large = 12,
    }
    public class PopupTextUI : MonoBehaviour
    {
        [SerializeField] private TextMeshPro text;
        [SerializeField] private float disappearTimer = 3f;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public async void Setup(Vector3 pos, string input, Color color, TextSize size = TextSize.Small)
        {
            gameObject.SetActive(true);
            transform.position = pos;
            text.text = input;
            text.color = color;
            text.fontSize = (int)size;
            await UniTask.Delay(TimeSpan.FromSeconds(disappearTimer));
            gameObject.SetActive(false);
        }

        private void OnValidate()
        {
            if (text == null) 
                text = GetComponent<TextMeshPro>();
        }
    }
}
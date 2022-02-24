using System;
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
        
        public void Setup(string input, Color color, TextSize size = TextSize.Small)
        {
            text.text = input;
            text.color = color;
            text.fontSize = (int)size;
        }

        private void OnValidate()
        {
            if (text == null) 
                text = GetComponent<TextMeshPro>();
        }
    }
}
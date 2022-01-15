using System;
using Managers;
using RockSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace UI
{
    public class BrushInput : MonoBehaviour
    {
        private void Update()
        { 
            TouchControl touch = Touchscreen.current?.primaryTouch;

            if (touch != null && touch.isInProgress)
            {
                Vector2 screenPosition = touch.position.ReadValue();
                Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
                
                Debug.Log($"hit at position {worldPosition}");
                M.GetOrNull<ChunkManager>()?.DamageChunk(worldPosition);
            }
        }
    }
}
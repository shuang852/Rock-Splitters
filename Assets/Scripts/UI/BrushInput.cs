using Managers;
using ToolSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace UI
{
    public class BrushInput : MonoBehaviour
    {
        private ToolManager toolManager;
        private Camera mainCamera;

        private bool alreadyDown;
        private bool alreadyUp;
        
        private void Start()
        {
            toolManager = M.GetOrNull<ToolManager>();
            
            mainCamera = Camera.main;
        }

        private void Update()
        {
            ToolUsage();
        }

        private void ToolUsage()
        {
            TouchControl touch = Touchscreen.current?.primaryTouch;

            if (touch == null) return;

            switch (touch.phase.ReadValue())
            {
                case TouchPhase.Began:
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (IsPointerOverUI())
                        break;

                    if (!alreadyDown)
                        ToolDown(touch);
                    else
                        ToolInUse(touch);
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (!alreadyUp)
                        ToolUp(touch);
                    break;
            }
        }

        private static bool IsPointerOverUI()
        {
            var touchId = Touchscreen.current.primaryTouch.touchId.ReadValue();
            return EventSystem.current.IsPointerOverGameObject(touchId);
        }

        private void ToolDown(TouchControl touch)
        {
            // Debug.LogWarning($"ToolDown: {touch.position.ReadValue()}");

            Vector2 worldPosition = GetTouchWorldPos(touch);

            toolManager.ToolDown(worldPosition);

            alreadyDown = true;
            alreadyUp = false;
        }

        private void ToolInUse(TouchControl touch)
        {
            // Debug.LogWarning($"ToolInUse: {touch.position.ReadValue()}");
            
            Vector2 worldPos = GetTouchWorldPos(touch);

            toolManager.ToolInUse(worldPos);
        }

        private void ToolUp(TouchControl touch)
        {
            // Debug.LogWarning($"ToolUp: {touch.position.ReadValue()}");
            
            Vector2 worldPos = GetTouchWorldPos(touch);

            toolManager.ToolUp(worldPos);

            alreadyDown = false;
            alreadyUp = true;
        }

        private Vector2 GetTouchWorldPos(TouchControl touch)
        {
            Vector2 screenPosition = touch.position.ReadValue();
            return mainCamera.ScreenToWorldPoint(screenPosition);
        }
    }
}
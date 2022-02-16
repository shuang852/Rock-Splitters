using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * Adapted from https://forum.unity.com/threads/unity-ui-scroll-through-items-one-by-one.294523/ by Ramsdal
 */
namespace UI.Generic
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollviewSnapping : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        // The content panel that

        [Tooltip("The content panel that holds the objects")]
        public Transform Panel;

        [SerializeField] private float stopVelocity = 3f;
        [SerializeField] private Button leftNavButton;
        [SerializeField] private Button rightNavButton;

        private List<Vector3> scrollPositions = new List<Vector3>();
        private ScrollRect scrollRect;
        private RectTransform scrollRectTransform;
        private bool isLerping;
        private bool isFindingClosest;
        private int index;

        private void Awake()
        {
            scrollRect = gameObject.GetComponent<ScrollRect>();
            scrollRectTransform = GetComponent<RectTransform>();
            isLerping = false;
            index = 0;
        }
 
        public void CalculateChildren()
        {
            scrollPositions.Clear();
            if (Panel.childCount > 0)
            {
                Panel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(
                    RectTransform.Axis.Horizontal, 
                    Panel.childCount * scrollRectTransform.rect.width);
                float imgSize = scrollRectTransform.rect.width;
                
                for (int i = 0; i < Panel.childCount; ++i)
                {
                    scrollPositions.Add(new Vector3((imgSize / 2 + i * imgSize) * -1, 0, 0f));
                }
                Panel.localPosition = scrollPositions[0];
            }
        }
 
        void Update()
        {
            if (isFindingClosest)
            {
                FindClosestFrom(Panel.localPosition);
            }

            if (index == 0)
            {
                leftNavButton.interactable = false;
                if (!rightNavButton.IsInteractable()) 
                    rightNavButton.interactable = true;
            } else if (index == scrollPositions.Count -1)
            {
                rightNavButton.interactable = false;
                if (!leftNavButton.IsInteractable()) 
                    leftNavButton.interactable = true;
            }
            else
            {
                if (!leftNavButton.IsInteractable()) 
                    leftNavButton.interactable = true;
                if (!rightNavButton.IsInteractable()) 
                    rightNavButton.interactable = true;
            }
            
            if (isLerping && scrollRect.velocity.magnitude < stopVelocity) 
            {
                Panel.localPosition = Vector3.Lerp(Panel.localPosition, scrollPositions[index], 10 * Time.deltaTime);

                if (Vector3.Distance(Panel.localPosition, scrollPositions[index]) < 0.001f)
                {
                    isLerping = false;
                    Panel.localPosition = scrollPositions[index];
                }
            }
        }
 
        private void FindClosestFrom(Vector3 start)
        {
            float distance = Mathf.Infinity;
 
            for (var i = 0; i < scrollPositions.Count; i++)
            {
                Vector3 pos = scrollPositions[i];
                if (Vector3.Distance(start, pos) < distance)
                {
                    distance = Mathf.Abs(Vector3.Distance(start, pos));
                    index = i;
                }
            }
        }

        #region DragControl
        
        public void OnDrag(PointerEventData data)
        {
            isLerping = false;
        }
        
        public void OnEndDrag(PointerEventData data)
        {
            if (scrollRect.horizontal)
            {
                isFindingClosest = true;
                isLerping = true;
            }
        }

        #endregion

        #region ButtonControl
       
        public void OnNextView()
        {
            index++;
            isFindingClosest = false;
            isLerping = true;
        }
        
        public void OnPreviousView()
        {
            index--;
            isFindingClosest = false;
            isLerping = true;
        }
        
        #endregion
    }
}
using System;
using System.Collections.Generic;
using DG.Tweening;
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
        [SerializeField] private float easeDuration = 1f;
        [SerializeField] private Button leftNavButton;
        [SerializeField] private Button rightNavButton;
        [SerializeField] private Ease easeMode;

        private List<Vector3> scrollPositions = new List<Vector3>();
        private ScrollRect scrollRect;
        private RectTransform scrollRectTransform;
        private bool isLerping;
        //private bool isTweening;
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
                isLerping = false;
                scrollRect.velocity = Vector2.zero;
                //isTweening = true;
                Panel.transform.DOLocalMoveX(scrollPositions[index].x, easeDuration, true)
                    .SetEase(easeMode);
                //.OnComplete(() => isTweening = false);
            }

            // TODO: FIX scroll elasticity not lining up. Drag beyond the content to see problem.
            // if (!isLerping && scrollRect.velocity.magnitude < stopVelocity && !isTweening && scrollPositions.Count > 0 
            //     && Math.Abs(Panel.localPosition.x - scrollPositions[index].x) > 0)
            // {
            //     Debug.Log("HALP");
            //     isTweening = true;
            //     Panel.transform.DOLocalMoveX(scrollPositions[index].x, easeDuration, true)
            //         .SetEase(easeMode)
            //         .OnComplete(() => isTweening = false);
            // }
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
            DOTween.Kill(Panel);
        }
        
        public void OnEndDrag(PointerEventData data)
        {
            isFindingClosest = true;
            isLerping = true;
        }

        #endregion

        #region ButtonControl
       
        public void OnNextView()
        {
            index++;
            Panel.transform.DOLocalMoveX(scrollPositions[index].x, easeDuration, true)
                .SetEase(easeMode);
        }
        
        public void OnPreviousView()
        {
            index--;
            Panel.transform.DOLocalMoveX(scrollPositions[index].x, easeDuration, true)
                .SetEase(easeMode);
        }
        
        #endregion
    }
}
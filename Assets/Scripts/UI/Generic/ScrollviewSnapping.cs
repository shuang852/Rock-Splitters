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

        private List<Vector3> scrollPositions = new List<Vector3>();
        private ScrollRect scrollRect;
        private RectTransform scrollRectTransform;
        private bool isLerping;
        private bool isFindingClosest;
        private int index;
        [SerializeField] private float stopVelocity = 3f;

        private void Awake()
        {
            scrollRect = gameObject.GetComponent<ScrollRect>();
            scrollRectTransform = GetComponent<RectTransform>();
            isLerping = false;
            index = 0;
            
            //scrollPositions = new List<Vector3>();
            //CalculateChildren();
 
            ((RectTransform)Panel).position.Set(0, 0, 0);
        }
 
        public void CalculateChildren()
        {
            scrollPositions.Clear();
            if (Panel.childCount > 0)
            {
                Panel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(
                    RectTransform.Axis.Horizontal, 
                    Panel.childCount * scrollRectTransform.rect.width);
                float imgSize = Panel.GetComponent<FlexibleGridLayout>().cellSize.x;
         
                for (int i = 0; i < Panel.childCount; ++i)
                {
                    scrollPositions.Add(new Vector3((imgSize / 2 + i * imgSize) * -1, 0, 0f));
                }
            }
        }
 
        void Update()
        {
            if (isFindingClosest)
            {
                FindClosestFrom(Panel.localPosition);
            }
            
            if (isLerping && scrollRect.velocity.magnitude < stopVelocity) 
            {
                Panel.localPosition = Vector3.Lerp(Panel.localPosition, scrollPositions[index], 10 * Time.deltaTime);

                if (Vector3.Distance(Panel.localPosition, scrollPositions[index]) < 0.001f)
                {
                    isLerping = false;
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
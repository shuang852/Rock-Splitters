using System;
using UnityEngine;
using UnityEngine.UI;


namespace UI.Generic
{
    /// <summary>
    /// This layout allows you to spread its children out in a grid layout.
    /// You can nest this object with more FGLs to create more complicated UI layouts.
    /// If you want the children to control their sizes, nest them under an empty parent. I.e. Layout > empty > UIObject.
    /// </summary>

    public enum FitType
    {
        Uniform,
        Width,
        Height,
        FixedRows,
        FixedColumns,
        FixedBoth
    }
    
    public class FlexibleGridLayout : LayoutGroup
    {
        [Tooltip("Set how you want the grid to look like. \n" +
                 "Uniform = let unity handle everything and spread children uniformly within the parent. \n" +
                 "Width = Fit the grid within the width. \n" +
                 "Height = Fit the grid within the height. \n" +
                 "Fixed Rows = Fit the grid within the rows you'll give. Columns is automatic. Can also control if it overflows with FitX & Y \n" +
                 "Fixed Columns = Fit the grid within the columns you'll give. Rows is automatic. Can also control if it overflows with FitX & Y \n" +
                 "Fixed Both = Control row and columns yourself. Can also control if it overflows with FitX & Y \n")]
        public FitType fitType;
        
        [Min(1)]
        public int rows;
        [Min(1)]
        public int columns;
        
        public Vector2 cellSize;
        public Vector2 spacing;

        [Tooltip("Have children fit within this object on the X Axis. Controllable with Fixed Fit types")]
        public bool fitX;
        [Tooltip("Have children fit within this object on the Y Axis. Controllable with Fixed Fit types")]
        public bool fitY;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            // TODO: Does FitType width or height do anything???
            if (fitType == FitType.Width || fitType == FitType.Height || fitType == FitType.Uniform)
            {
                fitX = true;
                fitY = true;
                float sqrRt = Mathf.Sqrt(transform.childCount);
                rows = Mathf.CeilToInt(sqrRt);
                columns = Mathf.CeilToInt(sqrRt);
            }
            
            switch (fitType)
            {
                case FitType.Width:
                case FitType.FixedColumns:
                    rows = Mathf.CeilToInt(transform.childCount / (float)columns);
                    break;
                case FitType.Height:
                case FitType.FixedRows:
                    columns = Mathf.CeilToInt(transform.childCount / (float)rows);
                    break;
                case FitType.FixedBoth:
                    break;
            }

            var rect = rectTransform.rect;
            float parentWidth = rect.width;
            float parentHeight = rect.height;

            float cellWidth = (parentWidth / columns) - ((spacing.x / columns) * (columns - 1)) - (padding.left / (float)columns) -
                              (padding.right / (float)columns);
            float cellHeight = (parentHeight / rows) - (spacing.y / rows * (rows - 1)) - (padding.top / (float)rows) -
                               (padding.bottom / (float)rows);
            
            cellSize.x = fitX ? cellWidth : cellSize.x;
            cellSize.y = fitY ? cellHeight : cellSize.y;

            for (int i = 0; i < rectChildren.Count; i++)
            {
                var rowCount = i / columns;
                var columnCount = i % columns;

                var item = rectChildren[i];

                var xPos = cellSize.x * columnCount + spacing.x * columnCount + padding.left;
                var yPos = cellSize.y * rowCount + spacing.y * rowCount + padding.top;
            
                SetChildAlongAxis(item, 0, xPos, cellSize.x) ;
                SetChildAlongAxis(item, 1, yPos, cellSize.y);
            }
        }

        public override void CalculateLayoutInputVertical()
        {
            
        }

        public override void SetLayoutHorizontal()
        {
        
        }

        public override void SetLayoutVertical()
        {
        
        }
    }
}
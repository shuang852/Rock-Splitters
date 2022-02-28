using System;
using Unity.Plastic.Antlr3.Runtime.Tree;
using UnityEngine;

namespace ToolSystem
{
    public class ToolVisualScaler : MonoBehaviour
    {
        [SerializeField] private Tool tool;
        [SerializeField] private bool manualOverride;
        
        private void OnValidate()
        {
            // TODO: Maths the radius since radius != 1:1 with hexes
            if (!manualOverride)
            {
                transform.localScale = new Vector3(tool.radius, tool.radius, 0);
            }
        }
    }
}
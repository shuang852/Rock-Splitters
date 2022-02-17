using UnityEditor;
using UnityEngine;
using Utility;

namespace Stored
{
    
    /* Adapted from Kryzarel's Items inventory tutorial
     */
    public class AssetDatabase <T> : ScriptableObject where T : Object
    {
        [SerializeField] private string assetPath;
        [SerializeField] protected T[] items;
#if UNITY_EDITOR
        protected void OnEnable()
        {
            EditorApplication.projectChanged -= LoadItems;
            EditorApplication.projectChanged += LoadItems;
        }

        protected void OnDisable()
        {
            EditorApplication.projectChanged -= LoadItems;
        }

        protected void LoadItems()
        {
            items = FindAssetsByGenericType.FindAssetsByType<T>(assetPath);
        }
#endif
    }
}
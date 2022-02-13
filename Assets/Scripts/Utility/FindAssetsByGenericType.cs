using UnityEditor;
using UnityEngine;

namespace Utility
{
    public static class FindAssetsByGenericType
    {
        // Slightly modified version of this answer: http://answers.unity.com/answers/1216386/view.html
#if UNITY_EDITOR
        public static T[] FindAssetsByType<T>(params string[] folders) where T : Object
        {
            string type = typeof(T).Name;

            string[] guids;
            if (folders == null || folders.Length == 0) {
                guids = AssetDatabase.FindAssets("t:" + type);
            } else {
                guids = AssetDatabase.FindAssets("t:" + type, folders);
            }

            T[] assets = new T[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                assets[i] = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }
            return assets;
        }
#endif
    }
}

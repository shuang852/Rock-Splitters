using Fossils;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/*
 * Created by Kryzarel from his Items inventory tutorial
 */

namespace Stored
{
    [CreateAssetMenu(menuName = "SO/Item Database")]
    public class AntiquitySetDatabase : ScriptableObject
    {
        [SerializeField] AntiquitySet[] items;

#if UNITY_EDITOR
        private void OnValidate()
        {
            LoadItems();
        }

        private void OnEnable()
        {
            EditorApplication.projectChanged -= LoadItems;
            EditorApplication.projectChanged += LoadItems;
        }

        private void OnDisable()
        {
            EditorApplication.projectChanged -= LoadItems;
        }

        private void LoadItems()
        {
            items = FindAssetsByType<AntiquitySet>("Assets/ScriptableObjects/AntiquitySets");
        }

        // Slightly modified version of this answer: http://answers.unity.com/answers/1216386/view.html
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
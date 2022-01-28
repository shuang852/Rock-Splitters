using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using System;
using UnityEditor;
#endif

/*
 * Created by Kryzarel from his Items inventory tutorial
 */

// TODO: Make this generic so it can support any type of object.
namespace Stored
{
    [CreateAssetMenu(menuName = "SO/Item Database")]
    public class AntiquitySetDatabase : ScriptableObject
    {
        [SerializeField] private AntiquitySet[] items;
        public AntiquitySet[] Items => items;
        public bool overrideOrder;
        [SerializeField] private AntiquitySet[] orderedItems;

#if UNITY_EDITOR
        private void OnValidate()
        {
            LoadItems();
            if (!overrideOrder)
            {
                CreateOrderedList();
            }
        }

        [ContextMenu("Create List")]
        private void CreateOrderedList()
        {
            orderedItems = items.OrderBy(set => set.ProdID).ToArray();
            // TODO: Verify there are no dupes. Output error if there is.
            // var dupes = orderedItems.GroupBy(x => x.ProdID)
            //     .Where(g => g.Count() > 1)
            //     .Select(y => new { Element = y.Key, Counter = y.Count()})
            //     .ToArray();
            //
            // if (dupes.Length <= 0) return;
            // foreach (var dupe in dupes)
            // {
            //     Debug.LogError($"{dupe.Element}");
            // }
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
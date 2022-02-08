using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using System;
using UnityEditor;
#endif

namespace Stored
{
    [CreateAssetMenu(menuName = "SO/DB/AntiquitySetDB")]
    public class AntiquitySetDatabase : AssetDatabase<AntiquitySet>
    {
        public AntiquitySet[] Items => items;
        public bool overrideOrder;
        [SerializeField] private AntiquitySet[] orderedItems;
        public AntiquitySet[] OrderedItems => orderedItems;
        
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
#endif
    }
}
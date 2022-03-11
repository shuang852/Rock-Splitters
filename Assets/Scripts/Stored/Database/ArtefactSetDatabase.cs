using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace Stored.Database
{
    [CreateAssetMenu(menuName = "SO/DB/ArtefactSetDB")]
    public class ArtefactSetDatabase : AssetDatabase<ArtefactSet>
    {
        public ArtefactSet[] Items => items;
        public bool overrideOrder;
        [SerializeField] private ArtefactSet[] orderedItems;
        public ArtefactSet[] OrderedItems => orderedItems;
        
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
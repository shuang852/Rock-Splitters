using System;
using Managers;
using UnityEngine;

namespace Stored
{
#if UNITY_EDITOR
    public class ItemSetTest : MonoBehaviour
    {
        [SerializeField] private Artefact testItem;
        private ArtefactManager manager;

        private void Start()
        {
            manager = M.GetOrThrow<ArtefactManager>();
        }
        
        [ContextMenu("Add Item Test")]
        public void AddItemTestF()
        {
            Debug.Log(manager.AddItem(testItem) ? $"Added {testItem} successfully" : $"Failed to add {testItem}");
        }
        
        [ContextMenu("Remove Item Test")]
        public void RemoveItemTestF()
        {
            Debug.Log(manager.RemoveItem(testItem) ? $"Added {testItem} successfully" : $"Failed to add {testItem}");
        }
    }
#endif
}
using System;
using Managers;
using UnityEngine;

namespace Stored
{
#if UNITY_EDITOR
    public class ItemSetTest : MonoBehaviour
    {
        [SerializeField] private Antiquity testItem;
        private AntiquityManager manager;

        private void Start()
        {
            manager = M.GetOrThrow<AntiquityManager>();
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
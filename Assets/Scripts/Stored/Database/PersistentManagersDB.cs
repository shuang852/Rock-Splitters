using Managers;
using UnityEditor;
using UnityEngine;

namespace Stored
{
    [CreateAssetMenu(fileName = "PersistentManagersDB", menuName = "SO/DB/PersistentManagersDB", order = 0)]
    public class PersistentManagersDB : AssetDatabase<GameObject>
    {
        public GameObject[] Items => items;
#if UNITY_EDITOR
        private void OnValidate() => LoadItems();
#endif
    }
}
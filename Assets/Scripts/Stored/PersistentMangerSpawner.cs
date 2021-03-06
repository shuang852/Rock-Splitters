using UnityEngine;

namespace Stored
{
    public class PersistentMangerSpawner : MonoBehaviour
    {
        [SerializeField] private PersistentManagersDB db;

        private void Awake()
        {
            foreach (var manager in db.Items)
            {
                if (GameObject.Find(manager.name + "(Clone)")) continue;
                
                Debug.Log($"Instantiating {manager.name}");
                Instantiate(manager);
            }
        }
    }
}

using System;
using UnityEngine;

namespace Stored
{
    public class PersistantMangerSpawner : MonoBehaviour
    {
        [SerializeField] private PersistentManagersDB db;

        private void Awake()
        {
            foreach (var manager in db.Items)
            {
                if (!GameObject.Find(manager.name))
                {
                    Instantiate(manager);
                }
            }
        }
    }
}

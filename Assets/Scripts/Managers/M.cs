using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public static class M
    {
        private static readonly Dictionary<Type, IManager> Managers = new Dictionary<Type, IManager>();

        /// <summary>
        /// Basically assume that we can get a manager of a certain type. Otherwise throw an exception.
        /// </summary>
        /// <exception cref="InvalidOperationException">No managers found for that type</exception>
        public static T Extort<T>() where T : IManager
        {
            T manager = GetOrNull<T>();

            if (manager == null)
            {
                throw new InvalidOperationException(
                    $"Trying to get manager {typeof(T).FullName}, but it is not ready!");
            }

            return manager;
        }

        /// <summary>
        /// Try to get a manager of that type, otherwise return null.
        /// </summary>
        public static T GetOrNull<T>() where T : IManager
        {
            return Managers.TryGetValue(typeof(T), out IManager manager) 
                ? (T) manager 
                : default;
        }

        /// <summary>
        /// Try to register a manager, may return false in cases where there is an existing manager and it cannot
        /// be replaced.
        /// </summary>
        public static void RegisterManager(IManager manager)
        {
            Type managerType = manager.GetType();

            // Check if there is an existing instance and it should be replaced
            if (Managers.ContainsKey(managerType))
            {
                IManager existingManager = Managers[managerType];

                // Ignore if the manager is already registered
                if (existingManager == manager) return;
                
                if (existingManager.CanBeReplaced && !existingManager.PersistBetweenScenes)
                {
                    Managers[managerType].OnReplaced(manager);
                }
                else
                {
                    GameObject.Destroy(manager.gameObject);
                    return;
                }
            }

            Managers[managerType] = manager;
            
            if (manager.PersistBetweenScenes && Application.IsPlaying(manager.gameObject))
                GameObject.DontDestroyOnLoad(manager.gameObject);
        }

        public static void UnregisterManager(IManager manager)
        {
            Type managerType = manager.GetType();

            // We need to make sure we don't remove an existing manager in the process of destroying the passed in manager
            if (Managers.TryGetValue(managerType, out IManager existingManager) && existingManager == manager)
            {
                Managers.Remove(manager.GetType());
            }
        }
    }
}
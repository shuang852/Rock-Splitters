using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Managers
{
    /// <summary>
    /// This is essentially a MANAGER LOCATOR but its more flexible.
    /// </summary>
    public static class M
    {
        private static readonly Dictionary<Type, IManager> managers = new Dictionary<Type, IManager>();
        private static readonly Dictionary<Type, Dictionary<UniTask, UnityEvent>> listeners =
            new Dictionary<Type, Dictionary<UniTask, UnityEvent>>();

        /// <summary>
        /// Basically assume that we can get a manager of a certain type. Otherwise it automatically throws an exception.
        ///
        /// <p>This should be the default when trying to get a manager</p>
        /// </summary>
        /// <exception cref="InvalidOperationException">No managers found for that type</exception>
        public static T GetOrThrow<T>() where T : IManager
        {
            return GetOrThrow<T>($"Trying to get manager {typeof(T).FullName}, but it is not ready!");
        }
        
        /// <summary>
        /// Basically assume that we can get a manager of a certain type. Otherwise throw an exception with an error message.
        /// </summary>
        public static T GetOrThrow<T>(string errorMessage) where T : IManager
        {
            return GetOrThrow<T>(new InvalidOperationException(errorMessage));
        }
        
        /// <summary>
        /// Basically assume that we can get a manager of a certain type. Otherwise throw an exception.
        /// </summary>
        public static T GetOrThrow<T>(Exception exception) where T : IManager
        {
            T manager = GetOrNull<T>();

            if (manager == null)
            {
                throw exception;
            }

            return manager;
        }

        public static async UniTask<T> GetOrWait<T>(CancellationToken token) where T : IManager
        {
            UniTask task = UniTask.CompletedTask;
            
            var managerType = typeof(T);
            
            try
            {
                if (managers.TryGetValue(managerType, out IManager manager))
                    return (T)manager;

                if (!listeners.ContainsKey(managerType))
                    listeners.Add(managerType, new Dictionary<UniTask, UnityEvent>());

                var newEvent = new UnityEvent();

                listeners[managerType].Add(UniTask.CompletedTask, newEvent);

                await newEvent.OnInvokeAsync(token);

                return GetOrThrow<T>();
            }
            catch (OperationCanceledException)
            {
                if (!listeners.ContainsKey(managerType))
                    throw new OperationCanceledException();
            
                if (!listeners[managerType].ContainsKey(task))
                    throw new OperationCanceledException();
            
                listeners[managerType][task].RemoveAllListeners();
                listeners[managerType].Remove(task);
                
                throw new OperationCanceledException();
            }
        }

        /// <summary>
        /// Try to get a manager of that type, otherwise return null.
        /// </summary>
        public static T GetOrNull<T>() where T : IManager
        {
            return managers.TryGetValue(typeof(T), out IManager manager) 
                ? (T) manager 
                : default;
        }

        /// <summary>
        /// Try to register a manager, may return false in cases where there is an existing manager and it cannot
        /// be replaced.
        ///
        /// <p>You can actually call this from anywhere at anytime, including before scene loads</p>
        /// </summary>
        public static void RegisterManager(IManager manager)
        {
            Type managerType = manager.GetType();

            // Check if there is an existing instance and it should be replaced
            if (managers.ContainsKey(managerType))
            {
                IManager existingManager = managers[managerType];

                // Ignore if the manager is already registered
                if (existingManager == manager) return;
                
                if (existingManager.CanBeReplaced && !existingManager.PersistBetweenScenes)
                {
                    managers[managerType].OnReplaced(manager);
                }
                else
                {
                    GameObject.Destroy(manager.gameObject);
                    return;
                }
            }

            managers[managerType] = manager;
            
            // Check for listeners
            if (listeners.ContainsKey(managerType))
            {
                foreach (var listenerDelegate in listeners[managerType].Values)
                {
                    listenerDelegate.Invoke();

                    listenerDelegate.RemoveAllListeners();
                }

                listeners.Remove(managerType);
            }
                
            
            if (manager.PersistBetweenScenes && Application.IsPlaying(manager.gameObject))
                GameObject.DontDestroyOnLoad(manager.gameObject);
        }

        public static void UnregisterManager(IManager manager)
        {
            Type managerType = manager.GetType();

            // We need to make sure we don't remove an existing manager in the process of destroying the passed in manager
            if (managers.TryGetValue(managerType, out IManager existingManager) && existingManager == manager)
            {
                managers.Remove(managerType);
                
                if (manager.PersistBetweenScenes)
                    SceneManager.MoveGameObjectToScene(manager.gameObject, SceneManager.GetActiveScene());
            }
        }
    }
}
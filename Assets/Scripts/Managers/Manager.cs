using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Managers
{
    public abstract class Manager : MonoBehaviour, IManager
    {
        public virtual bool CanBeReplaced => false;
        public virtual bool PersistBetweenScenes => false;

        private readonly Dictionary<Type, CancellationTokenSource> managerRequestCancellationSources = new Dictionary<Type, CancellationTokenSource>();
        
        void IManager.OnReplaced(object otherInstance)
        {
            if (otherInstance is Manager manager)
                OnReplaced(manager);
        }
        
        protected virtual void OnReplaced(Manager otherInstance) {}

        protected void RegisterManager()
        {
            M.RegisterManager(this);
        }

        protected virtual void OnEnable()
        {
            RegisterManager();
        }

        protected virtual void OnDisable()
        {
            M.UnregisterManager(this);
        }

        protected virtual void Awake()
        {
            // We want this in awake as well to be as early as possible
            RegisterManager();
        }

        protected virtual void Start()
        {
            // Cancel manager requests
            while (managerRequestCancellationSources.Keys.Count > 0)
            {
                var managerType = managerRequestCancellationSources.Keys.ToList()[0];
                
                managerRequestCancellationSources[managerType].Cancel();
                // TODO: Not sure if this is necessary...
                // managerRequestCancellationSources[managerType].Dispose();
            }

            managerRequestCancellationSources.Clear();
        }

        protected virtual void Update() {}

        protected virtual void FixedUpdate() {}

        protected virtual void OnDestroy() {}

        protected virtual void OnValidate() {}

        protected async UniTask<T> GetOrWait<T>() where T : IManager
        {
            try
            {
                Type managerType = typeof(T);

                CancellationTokenSource cancellationSource = new CancellationTokenSource();

                var task = M.GetOrWait<T>(cancellationSource.Token);

                if (managerRequestCancellationSources.ContainsKey(managerType))
                    throw new OperationCanceledException($"Already waiting for {managerType}");

                managerRequestCancellationSources.Add(managerType, cancellationSource);

                var manager = await task;

                managerRequestCancellationSources.Remove(managerType);

                return manager;
            }
            catch (OperationCanceledException)
            {
                managerRequestCancellationSources.Remove(typeof(T));
                
                throw new OperationCanceledException();
            }
        }
    }
}
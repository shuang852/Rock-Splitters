using UnityEngine;

namespace Managers
{
    public abstract class Manager : MonoBehaviour, IManager
    {
        public virtual bool CanBeReplaced => false;
        public virtual bool PersistBetweenScenes => false;
        
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
        
        protected virtual void Start() {}

        protected virtual void Update() {}

        protected virtual void FixedUpdate() {}

        protected virtual void OnDestroy() {}

        protected virtual void OnValidate() {}
    }
}
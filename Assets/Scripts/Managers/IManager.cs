using UnityEngine;

namespace Managers
{
    public interface IManager
    {
        // This is provided by Unity
        GameObject gameObject { get; }
        
        bool CanBeReplaced { get; }
        
        bool PersistBetweenScenes { get; }
        
        void OnReplaced(object otherInstance);
        
    }
}
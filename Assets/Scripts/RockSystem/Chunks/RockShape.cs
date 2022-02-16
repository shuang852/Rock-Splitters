using UnityEngine;

namespace RockSystem.Chunks
{
    [CreateAssetMenu]
    public class RockShape : ScriptableObject
    {
        public Sprite rockShapeMask;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
    }
}
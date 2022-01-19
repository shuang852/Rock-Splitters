using UnityEngine;

namespace Stored
{
    [CreateAssetMenu(fileName = "Antiquity", menuName = "Window/Scriptable Objects/Antiquity", order = 0)]
    public class Antiquity : ScriptableObject
    {
        [SerializeField] private string displayName;
        [SerializeField] private Sprite sprite;

        public string DisplayName => displayName;
        public Sprite Sprite => sprite;
    }
}
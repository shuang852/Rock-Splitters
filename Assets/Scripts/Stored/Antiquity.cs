using UnityEngine;

namespace Stored
{
    [CreateAssetMenu(fileName = "Antiquity", menuName = "Window/Scriptable Objects/Antiquity", order = 0)]
    public class Antiquity : ScriptableObject
    {
        [SerializeField] private string displayName;
        [SerializeField] private Sprite sprite;
        [Tooltip("The health at which this will start taking damage")]
        [SerializeField] private int breakingHealth;
        [Tooltip("The maximum damage this can take, as such the total health")]
        [SerializeField] private int maxHealth;

        public string DisplayName => displayName;
        public Sprite Sprite => sprite;
        public int BreakingHealth => breakingHealth;
        public int MaxHealth => maxHealth;
    }
}
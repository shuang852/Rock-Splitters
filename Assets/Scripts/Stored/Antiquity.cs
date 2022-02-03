using UnityEngine;

namespace Stored
{
    [CreateAssetMenu(fileName = "Antiquity", menuName = "Scriptable Objects/Antiquities/Antiquity", order = 0)]
    public class Antiquity : ScriptableObject
    {
        [SerializeField] private string displayName;
        [SerializeField] private Sprite sprite;
        [Tooltip("The health at which this will start taking damage")]
        [SerializeField] private int breakingHealth;
        [Tooltip("The maximum damage this can take, as such the total health")]
        [SerializeField] private int maxHealth;
        [Tooltip("THe final score you get after cleaning the fossil")]
        [SerializeField] private float score;
        [Tooltip("Set to true to manually choose its set. Otherwise it'll automatically be assigned a set if a set" +
                 "contains the antiquity.")]
        [SerializeField] private bool overrideSet;

        public string DisplayName => displayName;
        public Sprite Sprite => sprite;
        public int BreakingHealth => breakingHealth;
        public int MaxHealth => maxHealth;
        public bool OverrideSet => overrideSet;
        public float Score => score;
        public AntiquitySet AntiquitySet;
    }
}
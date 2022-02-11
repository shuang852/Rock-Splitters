using UnityEngine;
using UnityEngine.Serialization;

namespace Stored
{
    [CreateAssetMenu(fileName = "Artefact", menuName = "Scriptable Objects/Artefacts/Artefact", order = 0)]
    public class Artefact : ScriptableObject
    {
        [SerializeField] private string displayName;
        [SerializeField] private Sprite sprite;
        [Tooltip("The health at which this will start taking damage")]
        [SerializeField] private float breakingHealth;
        [Tooltip("The maximum damage this can take, as such the total health")]
        [SerializeField] private float maxHealth;
        [Tooltip("The final score you get after cleaning the fossil")]
        [SerializeField] private float score;
        [Tooltip("Set to true to manually choose its set. Otherwise it'll automatically be assigned a set if a set" +
                 "contains the artefact.")]
        [SerializeField] private bool overrideSet;

        public string DisplayName => displayName;
        public Sprite Sprite => sprite;
        public float BreakingHealth => breakingHealth;
        public float MaxHealth => maxHealth;
        public bool OverrideSet => overrideSet;
        public float Score => score;
        [FormerlySerializedAs("AntiquitySet")] public ArtefactSet artefactSet;
    }
}
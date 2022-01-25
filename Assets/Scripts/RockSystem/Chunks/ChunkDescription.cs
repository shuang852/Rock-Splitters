using UnityEngine;

namespace RockSystem.Chunks
{
    [CreateAssetMenu(fileName = "Chunk", menuName = "Chunk Description", order = 0)]
    public class ChunkDescription : ScriptableObject
    {
        [SerializeField] private Sprite sprite;
        [SerializeField] private Vector2 offset;
        [SerializeField] private Color damagedColor = Color.clear;
        [SerializeField] private int health;
        [Tooltip("A random number is generated between -X and X and added to the base chunk health.")]
        [SerializeField] private int healthVariation;

        public Vector2 Offset => offset;
        public Sprite Sprite => sprite;
        public int Health => health;
        public int HealthVariation => healthVariation;
    }
}
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

        public Vector2 Offset => offset;
        public Sprite Sprite => sprite;
        public int Health => health;
    }
}
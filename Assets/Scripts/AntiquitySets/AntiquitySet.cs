using Stored;
using UnityEngine;

namespace Fossils
{
    [CreateAssetMenu(fileName = "FossilSet", menuName = "Antiquities/AntiquitySet", order = 0)]
    public class AntiquitySet : ScriptableObject
    {
        [SerializeField] private string setName;
        [SerializeField] private Sprite sprite;
        [SerializeField] private Antiquity[] setItems;

        public string SetName => setName;
        public Sprite Sprite => sprite; 
        public Antiquity[] SetItems => setItems;
    }
}
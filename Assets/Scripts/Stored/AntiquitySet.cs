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
        [SerializeField] private float setIncomeRate;
        [SerializeField] private float setCapacity;
        [SerializeField] private float setBonus = 1.3f;

        public string SetName => setName;
        public Sprite Sprite => sprite; 
        public Antiquity[] SetItems => setItems;
        public float SetIncomeRate => setIncomeRate;
        public float SetCapacity => setCapacity;
        public float SetBonus => setBonus;
    }
}
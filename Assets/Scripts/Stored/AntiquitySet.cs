using System;
using UnityEngine;

namespace Stored
{
    [CreateAssetMenu(fileName = "FossilSet", menuName = "Antiquities/AntiquitySet", order = 0)]
    public class AntiquitySet : ScriptableObject
    {
        [SerializeField] private string setName;
        [SerializeField] private Sprite sprite;
        [Tooltip("The items within the set. Ensure the order is correct as they will be displayed TOP to BOT. Go from Head to legs")]
        [SerializeField] private Antiquity[] setItems;
        [SerializeField] private float baseSetIncomeRate;
        [SerializeField] private float baseSetCapacity;
        [SerializeField] private float setBonus = 1.3f;

        public string SetName => setName;
        public Sprite Sprite => sprite; 
        public Antiquity[] SetItems => setItems;
        public float BaseSetIncomeRate => baseSetIncomeRate;
        public float BaseSetCapacity => baseSetCapacity;
        public float SetBonus => setBonus;
        
        // Non editor variables 
        public int Count { get; private set; }
        public bool BonusActive { get; private set; } // Could use this to display something when set is completed
        public float CurrentSetIncomeRate { get; private set; }
        public float CurrentSetCapacity { get; private set; }

        private void OnValidate()
        {
            foreach (var item in setItems)
                if (!item.OverrideSet) item.AntiquitySet = this;
        }

        // TODO: Possibly change this to only 1 item if we face optimisation problems. This would mean a version without resetting count
        // TODO: Another possible optimisation is only adding or removing not both
        /// <summary>
        /// Check if the player has the sets items then returns the income rate and capacity.
        /// </summary>
        /// <param name="inventory">Inventory containing antiquities</param>
        /// <returns>The set's current income rate and capacity calculated after checking how many set items exist</returns>
        public void ValidateSet(Inventory inventory)
        {
            Count = 0;
            foreach (var item in setItems)
            {
                if (inventory.Contains(item))
                {
                    Count++;
                }
            }

            float unlockedPercentage = (float)Count / SetItems.Length;
            float bonus;
            
            if (unlockedPercentage >= 1)
            {
                BonusActive = true;
                bonus = setBonus;
            }
            else
            {
                BonusActive = false;
                bonus = 1f;
            }
            
            CurrentSetIncomeRate = unlockedPercentage * bonus * BaseSetIncomeRate;
            CurrentSetCapacity = unlockedPercentage * bonus * BaseSetCapacity;
        }
    }
}
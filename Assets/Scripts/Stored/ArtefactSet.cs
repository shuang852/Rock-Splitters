using UnityEngine;

namespace Stored
{
    [CreateAssetMenu(fileName = "ArtefactSet", menuName = "Scriptable Objects/Artefacts/ArtefactSet", order = 0)]
    public class ArtefactSet : ScriptableObject
    {
        [SerializeField] private string setName;
        [SerializeField] private Sprite sprite;
        [SerializeField] private int prodID;
        [Tooltip("Description of the set")][Multiline]
        [SerializeField] private string description;

        [Tooltip("The items within the set. Ensure the order is correct as they will be displayed TOP to BOT. Go from Head to legs")]
        [SerializeField] private Artefact[] setItems;

        // [Tooltip("The amount of money you gain per hour")]
        // [SerializeField] private float baseSetIncome;
        //
        // [Tooltip("The amount of money you can gain offline until it stops. Usually 24 times the income")]
        // [SerializeField] private float baseSetCapacity;
        //
        // [Tooltip("Multiplier for income when entire set is collected. Default 1.3f")]
        // [SerializeField] private float setBonus = 1.3f;

        public string SetName => setName;
        public Sprite Sprite => sprite;
        public int ProdID => prodID;
        public string Description => description;
        public Artefact[] SetItems => setItems;
        // public float BaseSetIncome => baseSetIncome;
        // public float BaseSetCapacity => baseSetCapacity;
        // public float SetBonus => setBonus;

        // Non editor variables 
        public int Count { get; private set; }
        // public bool BonusActive { get; private set; } // Could use this to display something when set is completed
        // public float CurrentSetIncome { get; private set; }
        // public float CurrentSetCapacity { get; private set; }

        private void OnValidate()
        {
            if (setItems.Length <= 0) return;
            
            foreach (var item in setItems)
                if (item is { OverrideSet: false })
                    item.artefactSet = this;
        }

        // TODO: Possibly change this to only 1 item if we face optimisation problems. This would mean a version without resetting count
        // TODO: Another possible optimisation is only adding or removing not both
        /// <summary>
        /// Check if the player has the sets items then returns the income rate and capacity.
        /// </summary>
        /// <param name="inventory">Inventory containing artefacts</param>
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

            
            // Below is income calculations
            // float unlockedPercentage = (float)Count / SetItems.Length;
            // float bonus;
            
            // if (unlockedPercentage >= 1)
            // {
            //     BonusActive = true;
            //     bonus = setBonus;
            // }
            // else
            // {
            //     BonusActive = false;
            //     bonus = 1f;
            // }
            //
            // CurrentSetIncome = unlockedPercentage * bonus * BaseSetIncome;
            // CurrentSetCapacity = unlockedPercentage * BaseSetCapacity;
        }
    }
}
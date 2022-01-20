using Managers;
using UnityEngine;

namespace Stored
{
    public class AntiquityManager : Manager
    {
        private Inventory antiquityInventory = new Inventory(100);
        public Inventory AntiquityInventory => antiquityInventory;
        public override bool PersistBetweenScenes => true;

        public float AntiquityIncome { private set;  get; }
        public float AntiquityCapacity { private set;  get; }
        
        protected override void Start()
        {
            base.Start();
        }

        public void AddSetStats(float income, float capacity)
        {
            AntiquityIncome += income;
            AntiquityCapacity += capacity;
        }
        
        // TODO: Figure if you can sell and how to sell it
        // public bool RemoveItem(Antiquity item)
        // {
        //     return antiquityInventory.RemoveItem(item);
        // }
    }
}

using Managers;
using UnityEngine;

namespace Stored
{
    public class AntiquityManager : Manager
    {
        private Inventory antiquityInventory = new Inventory(100);
        public Inventory AntiquityInventory => antiquityInventory;
        public override bool PersistBetweenScenes => true;
        
        protected override void Start()
        {
            base.Start();
        }
        
        // TODO: Figure if you can sell and how to sell it
        // public bool RemoveItem(Antiquity item)
        // {
        //     return antiquityInventory.RemoveItem(item);
        // }
    }
}

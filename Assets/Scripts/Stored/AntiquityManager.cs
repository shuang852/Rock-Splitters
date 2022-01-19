using Managers;
using UnityEngine;

namespace Stored
{
    public class AntiquityManager : Manager
    {
        private Inventory antiquityInventory = new Inventory(1000);
        public Inventory AntiquityInventory => antiquityInventory;
        
        // TODO: Figure if you can sell and how to sell it
        // public bool RemoveItem(Antiquity item)
        // {
        //     return antiquityInventory.RemoveItem(item);
        // }
    }
}

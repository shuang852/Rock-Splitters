using System;
using System.Collections.Generic;

namespace Stored
{
    [Serializable]
    public class InventorySaveData
    {
        public List<Artefact> artefacts;

        public InventorySaveData(List<Artefact> artefacts)
        {
            this.artefacts = artefacts;
        }
    }
}
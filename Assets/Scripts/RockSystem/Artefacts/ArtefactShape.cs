using RockSystem.Chunks;
using Stored;
using UnityEngine;
using UnityEngine.Serialization;

namespace RockSystem.Artefacts
{
    public class ArtefactShape : ChunkShape
    {
        [FormerlySerializedAs("fossil")] [SerializeField] private Artefact artefact;

        public Artefact Artefact => artefact;

        public void Initialise(Artefact artefact)
        {
            this.artefact = artefact;
            
            // TODO: Properly initialise layer.
            Initialise(artefact.Sprite, artefact.MaxHealth, 0);
        }
    }
}
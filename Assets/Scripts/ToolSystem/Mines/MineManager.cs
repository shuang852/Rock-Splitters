using RockSystem.Chunks;
using UnityEngine;

namespace ToolSystem.Mines
{
    public class MineManager : ChunkShapeManager<Mine>
    {
        [SerializeField] private GameObject minePrefab;

        public void Initialise(int minesToGenerate)
        {
            base.Initialise();
            
            for (int i = 0; i < minesToGenerate; i++)
            {
                var go = Instantiate(minePrefab, transform);
                ChunkShapeGameObjects.Add(go);

                var mine = go.GetComponent<Mine>();

                // TODO: Properly initalise layer
                mine.Initialise(3);
                
                RegisterArtefact(mine);
            }
        }
    }
}
using System.Collections.Generic;
using Managers;
using RockSystem.Chunks;
using UnityEngine;

namespace ToolSystem.Mines
{
    public class MineManager : Manager
    {
        [SerializeField] private GameObject minePrefab;
        
        // TODO: Make sure when new mines are registered their CanBeDamaged is set correctly 
        public bool MinesCanBeDamaged
        {
            get => minesCanBeDamaged;
            set
            {
                mines.ForEach(a => a.CanBeDamaged = value);
                
                minesCanBeDamaged = value;
            }
        }
        
        private readonly List<GameObject> mineGameObjects = new List<GameObject>();
        private readonly List<Mine> mines = new List<Mine>();

        private ChunkManager chunkManager;
        private bool minesCanBeDamaged = true;

        protected override void Start()
        {
            base.Start();

            chunkManager = M.GetOrThrow<ChunkManager>();
        }

        public void Initialise(int minesToGenerate)
        {
            Deinitialise();
            
            for (int i = 0; i < minesToGenerate; i++)
            {
                var go = Instantiate(minePrefab, transform);
                mineGameObjects.Add(go);

                var mine = go.GetComponent<Mine>();
                mines.Add(mine);
                
                // TODO: Properly initalise layer
                mine.Initialise(3);
                
                // TODO: Should this just be handled by the ChunkShape? Leads to two way dependency
                chunkManager.RegisterChunkShape(mine);
            }
        }

        private void Deinitialise()
        {
            mineGameObjects.ForEach(Destroy);
            
            mineGameObjects.Clear();
            mines.Clear();
        }
        
        // TODO: Deuplicate code. See ArtefactShapeManager.CheckHealthAndExposure
        public void CheckHealthAndExposure()
        {
            bool healthChanged = false;
            bool exposureChanged = false;
            
            foreach (var mine in mines)
            {
                if (mine.CheckHealth()) healthChanged = true;

                if (mine.CheckExposure()) exposureChanged = true;
            }
            
            // if (healthChanged)
                // artefactDamaged.Invoke();

            // if (exposureChanged)
                // artefactExposed.Invoke();
        }
    }
}
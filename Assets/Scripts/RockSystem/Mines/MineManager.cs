using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace RockSystem.Mines
{
    public class MineManager : Manager
    {
        private readonly List<GameObject> mineGameObjects = new List<GameObject>();
        private readonly List<Mine> mines = new List<Mine>();
        
        public void Initialise(int minesToGenerate)
        {
            Deinitialise();
            
            for (int i = 0; i < minesToGenerate; i++)
            {
                GameObject go = new GameObject($"Mine");
                go.transform.parent = transform;
                Mine mine = go.AddComponent<Mine>();
                
                mineGameObjects.Add(go);
                mines.Add(mine);
            }
        }

        private void Deinitialise()
        {
            mineGameObjects.ForEach(Destroy);
            
            mineGameObjects.Clear();
            mines.Clear();
        }
    }
}
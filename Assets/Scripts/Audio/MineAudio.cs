using Managers;
using ToolSystem.Mines;
using UnityEngine;

namespace Audio
{
    public class MineAudio : MonoBehaviour
    {
        [SerializeField] private PlayOneShot defuse;
        [SerializeField] private PlayOneShot detonate;
        private MineManager mineManager;

        private void Start()
        {
            mineManager = M.GetOrThrow<MineManager>();

            mineManager.mineDetonated.AddListener(PlayDetonated);
            mineManager.mineDefused.AddListener(PlayDefuse);
        }

        private void PlayDefuse(Mine mine) => defuse.PlayOnce();
        
        private void PlayDetonated(Mine mine) => detonate.PlayOnce();

        private void OnDestroy()
        {
            mineManager.mineDetonated.RemoveListener(PlayDetonated);
            mineManager.mineDefused.RemoveListener(PlayDefuse);
        }
    }
}
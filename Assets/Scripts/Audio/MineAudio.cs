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

        private void PlayDefuse() => defuse.PlayOnce();
        
        private void PlayDetonated() => detonate.PlayOnce();
    }
}
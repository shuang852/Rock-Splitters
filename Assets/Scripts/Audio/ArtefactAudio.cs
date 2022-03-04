using System;
using Cleaning;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using Managers;
using RockSystem.Artefacts;
using Stored;
using UnityEngine;

namespace Audio
{
    public class ArtefactAudio : MonoBehaviour
    {
        [SerializeField] private PlayOneShot hitArtefact;
        // TODO: Change this SFX
        [SerializeField] private PlayOneShot hitBrokenArtefact;

        private CleaningManager cleaningManager;
        private ArtefactShapeManager artefactShapeManager;
        private Artefact Artefact => cleaningManager.CurrentArtefactRock.Artefact;

        private void Start()
        {
            cleaningManager = M.GetOrThrow<CleaningManager>();
            artefactShapeManager = M.GetOrThrow<ArtefactShapeManager>();

            artefactShapeManager.artefactChunkDamaged.AddListener(PlayAudio);
        }

        private void PlayAudio(ArtefactShape artefactShape, Vector2Int pos)
        {
            float remainingHealth = artefactShape.GetChunkHealth(pos);
            float damagePercentage = 1f - (remainingHealth / Artefact.MaxHealth);
            if (damagePercentage < 1)
            {
                if (hitBrokenArtefact.PlaybackState == PLAYBACK_STATE.PLAYING)
                    return;
                
                if (hitArtefact.PlaybackState != PLAYBACK_STATE.PLAYING)
                {
                    hitArtefact.PlayOnce();
                }
            }
            else
            {
                if (hitBrokenArtefact.PlaybackState != PLAYBACK_STATE.PLAYING)
                {
                    hitBrokenArtefact.PlayOnce();
                }
            }
        }
    }
}
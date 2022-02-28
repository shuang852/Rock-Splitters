using System;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Audio
{

    public class BGMChanger : MonoBehaviour
    {
        private string previousLevel;
        private string currentLevel;

        //TODO use scene reference instead of string
        public GenericDictionary<string, FMODUnity.EventReference> musicCatalogue;
        private static FMOD.Studio.EventInstance instance;

        private void Start()
        {
            previousLevel = SceneManager.GetActiveScene().name;
            // currentLevel = previousLevel;
            // var music = musicCatalogue[currentLevel];
            // instance = FMODUnity.RuntimeManager.CreateInstance(music);
            // instance.start();
            // instance.release();
        }

        private void ChangeMusic(Scene scene, LoadSceneMode mode)
        {
            currentLevel = scene.name;
            if (currentLevel == previousLevel) return;
            if (previousLevel != null)
            {
                if (musicCatalogue[currentLevel].ToString() == 
                    musicCatalogue[previousLevel].ToString())
                    return;
            }

            previousLevel = currentLevel;
            instance.stop(STOP_MODE.ALLOWFADEOUT);
            instance.release();
            var music = musicCatalogue[currentLevel];
            instance = FMODUnity.RuntimeManager.CreateInstance(music);
            instance.start();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += ChangeMusic;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= ChangeMusic;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using FMOD;
using FMOD.Studio;
using UnityEngine;

public class FmodExampleScript : MonoBehaviour
{
    public FMODUnity.EventReference test;
    
    private FMOD.Studio.EventInstance instance;

    private FMOD.Studio.PARAMETER_ID paramId; // Use an cached ID instead of a string. More performant and allows easier reuse

    [Range(-12,12)]
    public float pitch;

    // TODO: Move this to a utility class for audio to get the parameter by ID.
    private PARAMETER_ID GetParameterIDByName(EventInstance instance, string paramName)
    {
        instance.getDescription(out var eventDescription);
        eventDescription.getParameterDescriptionByName(name, out var paramDescription);
        return paramDescription.id;
    }
    
    void Start()
    {
        /*
         * Use this instance if we want to use 1 instance but reuse it. This will force the audio to cut itself when
         * repeated.
         */
        instance = FMODUnity.RuntimeManager.CreateInstance(test);

        /*
         * General form of getting param ID 
         */
        instance.getDescription(out var pitchEventDescription);
        pitchEventDescription.getParameterDescriptionByName("TestParam", out var pitchParamDescription);
        paramId = pitchParamDescription.id;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
             /*
              * Use this instance if we want to create multiple instances that overlap their audio
              * Start plays the audio
              * Release will set the instance to be destroyed once it has stopped. Good for oneshots.
              */
            instance = FMODUnity.RuntimeManager.CreateInstance(test);
           
            instance.start();
            instance.release();
            
            /*
             * Sets a float parameter using string
             * Sets a float parameter using a parameter ID
             * Sets a label/string parameter using a string 
             * Sets a label/string parameter using a parameter ID
             * Sets a float global parameter using a string. Can be used with labels if you change it to a label
             * Sets a float global parameter using an ID
             */
            //instance.setParameterByName("Reverb", vol);
            instance.setParameterByID(paramId, pitch);
            //EventInstance.setParameterByNameWithLabel("MusicState", "Battle");
            //instance.setParameterByIDWithLabel(paramId, "battle");
            //FMODUnity.RuntimeManager.StudioSystem.setParameterByName("paramName", value);
            //FMODUnity.RuntimeManager.StudioSystem.setParameterByID("paramName", value);
        }
    }
}

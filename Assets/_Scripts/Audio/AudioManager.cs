using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

namespace GGJ24
{
    public sealed class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        private EventInstance ambientEventInstance;

        private void Awake()
        {
            if (Instance == null)
            {
                Debug.LogWarning("Found more than one AudioManager Instance11");
            }
            Instance = this;
        }

        private void Start()
        {
            InitializeAmbiance(FMODEvents.Instance.Ambiance);
        }

        private void InitializeAmbiance(EventReference ambientEventReference)
        {
            ambientEventInstance = CreateEventInstance(ambientEventReference);
            ambientEventInstance.start();
        }

        public void SetAmbianceParameter(string parameterName, int parameterValue)
        {
            ambientEventInstance.setParameterByName(parameterName, parameterValue);
            Debug.Log("Set ambiance intensity to " + parameterValue);
        }

        public void PlayOneShot(EventReference sound, Vector3 worldPosition)
        {
            RuntimeManager.PlayOneShot(sound, worldPosition);
        }

        public EventInstance CreateEventInstance(EventReference eventReference)
        {
            EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
            return eventInstance;
        }
    }
}
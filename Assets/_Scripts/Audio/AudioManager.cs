using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.UIElements;

namespace GGJ24
{
    public sealed class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        private EventInstance ambientEventInstance;

        [Header("Change music intensity depending on time remaining")]
        [SerializeField] private float _intensityLow = 25;
        [SerializeField] private float _intensityMid = 15;
        [SerializeField] private float _intensityHigh = 10;
        float[] _thresholdTimes;


        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Found more than one AudioManager Instance11");
            }
            Instance = this;

            // Must be sorted in decreasing order of value
            _thresholdTimes = new float[] { _intensityLow, _intensityMid, _intensityHigh };
        }

        public void StartAmbiance()
        {
            ambientEventInstance = CreateEventInstance(FMODEvents.Instance.Ambiance);
            ambientEventInstance.start();
            InvokeRepeating(nameof(CheckAndUpdateMusicIntensity), 0f, 1f);
        }

        private void CheckAndUpdateMusicIntensity()
        {
            float remainingTime = GameManager.Instance.RemainingTime;
            int minIntensity = 0;
            SetAmbianceParameter("Intensity", minIntensity);

            foreach (float threshold in _thresholdTimes)
            {
                if (remainingTime < threshold)
                {
                    SetAmbianceParameter("Intensity", ++minIntensity);
                }
            }
        }

        private void UpdateMusicIntensity(float threshold)
        {
            SetAmbianceParameter("Intensity", 3);
        }

        public void StopAmbiance()
        {
            ambientEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }

        public void SetAmbianceParameter(string parameterName, int parameterValue)
        {
            ambientEventInstance.setParameterByName(parameterName, parameterValue);
        }

        public void PlayOneShot(EventReference sound, Vector3 worldPosition)
        {
            RuntimeManager.PlayOneShot(sound, worldPosition);
        }

        public EventInstance PlayStoppablOneShot(EventReference sound, Vector3 worldPosition)
        {
            EventInstance instance = RuntimeManager.CreateInstance(sound);
            instance.set3DAttributes(RuntimeUtils.To3DAttributes(worldPosition));
            instance.start();
            instance.release();
            return instance;
        }

        public EventInstance CreateEventInstance(EventReference eventReference)
        {
            EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
            return eventInstance;
        }

        public EventInstance CreateAttachedEventInstance(EventReference eventReference, Transform transform)
        {
            EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
            eventInstance.start(); // must start first before attaching
            RuntimeManager.AttachInstanceToGameObject(eventInstance, transform);
            return eventInstance;
        }
    }
}
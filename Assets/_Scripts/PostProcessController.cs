using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GGJ24
{
    public class PostProcessController : MonoBehaviour
    {
        public static PostProcessController Instance { get; private set; }

        private Volume _postProcessVolume;
        private Vignette _vignette;
        //private Bloom _bloom;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Found more than one VignetteController Instance");
            }
            Instance = this;
            _postProcessVolume = GetComponent<Volume>();
        }

        private void Start()
        {
            if (!_postProcessVolume.profile.TryGet<Vignette>(out _vignette))
            {
                Debug.LogWarning("Vignette effect not found in the PostProcessVolume.");
            }
        }

        //public void SetBloomIntensity(float intensity)
        //{
        //    if (_vignette != null)
        //    {
        //        _bloom.intensity.value = intensity;
        //    }
        //    else
        //    {
        //        Debug.LogWarning("Vignette effect not found or not initialized.");
        //    }
        //}

        public void SetVignetteIntensity(float intensity)
        {
            if (_vignette != null)
            {
                _vignette.intensity.value = intensity;
            }
            else
            {
                Debug.LogWarning("Vignette effect not found or not initialized.");
            }
        }
    }
}
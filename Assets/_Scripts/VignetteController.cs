using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GGJ24
{
    public class VignetteController : MonoBehaviour
    {
        [SerializeField] private Volume postProcessVolume;
        private Vignette vignette;

        private void Start()
        {
            if (!postProcessVolume.profile.TryGet<Vignette>(out vignette))
            {
                Debug.LogWarning("Vignette effect not found in the PostProcessVolume.");
            }
        }

        public void SetVignetteIntensity(float intensity)
        {
            if (vignette != null)
            {
                vignette.intensity.value = intensity;
            }
            else
            {
                Debug.LogWarning("Vignette effect not found or not initialized.");
            }
        }
    }
}
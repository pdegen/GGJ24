using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

namespace GGJ24
{
    public class WaterEffects : MonoBehaviour
    {
        [SerializeField] private GameObject _waterRippleEffect;
        [SerializeField] private GameObject _waterSplashEffect;
        [SerializeField] private GameObject _waterSplashEffectSmall;

        public void SpawnRipple(Vector3 position)
        {
            GameObject ripple = Instantiate(_waterRippleEffect, new Vector3(position.x, GameParamsLoader.WaterLevel, position.z), Quaternion.identity);
            ripple.transform.localScale *= 0.3f;
            ripple.transform.DOScale(3f, 3f);
            Destroy(ripple, 3f);
        }

        public void SpawnSplash(Vector3 position, float scale = 1)
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.SplashSFX, transform.position);
            GameObject splash = Instantiate(_waterSplashEffect, new Vector3(position.x, GameParamsLoader.WaterLevel, position.z), Quaternion.identity);
            splash.transform.localScale = new Vector3(scale, scale, scale);
            Destroy(splash, 3f);
        }

        public void SpawnBigSplash(Vector3 position)
        {
            SpawnSplash(position, 3);
            GameObject splash2 = Instantiate(_waterSplashEffectSmall, new Vector3(position.x+1, GameParamsLoader.WaterLevel, position.z-1), Quaternion.identity);
            GameObject splash3 = Instantiate(_waterSplashEffectSmall, new Vector3(position.x-1, GameParamsLoader.WaterLevel, position.z+1), Quaternion.identity);
            GameObject splash4 = Instantiate(_waterSplashEffectSmall, new Vector3(position.x+1, GameParamsLoader.WaterLevel, position.z-1), Quaternion.identity);
            GameObject splash5 = Instantiate(_waterSplashEffectSmall, new Vector3(position.x-1, GameParamsLoader.WaterLevel, position.z+1), Quaternion.identity);
            GameObject splash6 = Instantiate(_waterSplashEffectSmall, new Vector3(position.x+1, GameParamsLoader.WaterLevel, position.z-1), Quaternion.identity);
            Destroy(splash2, 3f);
            Destroy(splash3, 3f);
            Destroy(splash4, 3f);
            Destroy(splash5, 3f);
            Destroy(splash6, 3f);
        }
    }
}
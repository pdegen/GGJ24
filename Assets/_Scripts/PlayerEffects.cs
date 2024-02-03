using DG.Tweening;
using GGJ24;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    private ThirdPersonController _playerController;
    [SerializeField] private GameObject _waterRippleEffect;
    [SerializeField] private GameObject _waterSplashEffect;
    private bool _isInWater;
    [SerializeField] private float _ripplePeriod = 0.16f;
    private float _rippleTimer;

    private void Awake()
    {
        _isInWater = false;
    }

    private void Start()
    {
        _playerController = GetComponentInParent<ThirdPersonController>();
    }

    private void Update()
    {
        HandleWater();
    }

    private void HandleWater()
    {
        if (!_isInWater && _playerController.transform.position.y < GameParamsLoader.WaterLevel)
        {
            _isInWater = true;
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.SplashSFX, transform.position);
            GameObject splash = Instantiate(_waterSplashEffect, new Vector3(_playerController.transform.position.x, GameParamsLoader.WaterLevel, _playerController.transform.position.z), Quaternion.identity);
            Destroy(splash, 3f);
        }
        else if (_isInWater)
        {
            if (_playerController.transform.position.y > GameParamsLoader.WaterLevel)
            {
                _isInWater = false;
                return;
            }

            if (_playerController.CurrentSpeed < 0.5f) return;

            _rippleTimer += Time.deltaTime;

            if (_ripplePeriod / _playerController.CurrentSpeed < _rippleTimer)
            {
                GameObject ripple = Instantiate(_waterRippleEffect, new Vector3(_playerController.transform.position.x, GameParamsLoader.WaterLevel, _playerController.transform.position.z), Quaternion.identity);
                _rippleTimer = 0;
                Destroy(ripple, 2f);
            }
        }

    }
}

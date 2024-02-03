using DG.Tweening;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    private ThirdPersonController _playerController;
    [SerializeField] private GameObject _waterRippleEffect;
    [SerializeField] private Transform _pond;
    private float _waterLevel;
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
        _waterLevel = _pond.position.y;
    }

    private void Update()
    {
        HandleWater();
    }

    private void HandleWater()
    {
        if (!_isInWater && _playerController.transform.position.y < _waterLevel)
        {
            _isInWater = true;
            GameObject splash = Instantiate(_waterSplashEffect, new Vector3(_playerController.transform.position.x, _waterLevel, _playerController.transform.position.z), Quaternion.identity);
            Destroy(splash, 3f);
        }
        else if (_isInWater)
        {
            if (_playerController.transform.position.y > _waterLevel)
            {
                _isInWater = false;
                return;
            }

            if (_playerController.CurrentSpeed < 0.5f) return;

            _rippleTimer += Time.deltaTime;

            if (_ripplePeriod / _playerController.CurrentSpeed < _rippleTimer)
            {
                GameObject ripple = Instantiate(_waterRippleEffect, new Vector3(_playerController.transform.position.x, _waterLevel, _playerController.transform.position.z), Quaternion.identity);
                _rippleTimer = 0;
                Destroy(ripple, 2f);
            }
        }

    }
}

using DG.Tweening;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ24
{
    public class PlayerEffects : MonoBehaviour
    {
        private ThirdPersonController _playerController;
        [SerializeField] private WaterEffects _waterEffects;
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
            if (_waterEffects == null) Debug.LogError("No water effects found");
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
                _waterEffects.SpawnSplash(transform.position);
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
                    _waterEffects.SpawnRipple(transform.position);
                    _rippleTimer = 0;
                }
            }

        }
    }
}
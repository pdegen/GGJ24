using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;
using UnityEngine.UI;

namespace GGJ24
{
    [RequireComponent(typeof(NavMeshAgent), typeof(AgentMovement), typeof(Targeting))]
    public class Tank : MonoBehaviour, IDamageable
    {

        [SerializeField] private float _awakenDuration = 10f;
        private NavMeshAgent _agent;
        private AgentMovement _movement;
        private Shooting _shooting;
        private Targeting _targeting;
        private bool _isActive;
        [SerializeField] private GameObject _canvas;
        private Slider _healthSlider;

        [SerializeField] private float _maxHealth;
        [SerializeField] private GameObject _deathEffect;
        private float _currentHealth;

        public Vector3 Position => transform.position;

        private void Awake()
        {
            _movement = GetComponent<AgentMovement>();
            _movement.enabled = false;
            _targeting = GetComponent<Targeting>();
            _targeting.TargetingEnabled = false; // don't do _targeting.enabled = false else AgentMovement rotate towards target will fail
            _agent = GetComponent<NavMeshAgent>();
            _agent.enabled = false;
            _isActive = false;
            _canvas.SetActive(false);
            _currentHealth = _maxHealth;
            _healthSlider = _canvas.GetComponentInChildren<Slider>();
            _healthSlider.maxValue = _maxHealth;
            _healthSlider.value = _maxHealth;
        }

        private void Start()
        {
            _shooting = _targeting.Shooting;
            _shooting.CanShoot = false;
        }

        private void OnEnable()
        {
            Egg.CollectedEgg += ActivateTankWrapper;
        }

        private void OnDisable()
        {
            Egg.CollectedEgg -= ActivateTankWrapper;
        }

        //private void Update()
        //{
        //    if (!_isActive) { return; }
        //}

        private void ActivateTankWrapper()
        {
            if (EggManager.CollectedEggs != GameParamsLoader.EggsCollectedToSpawnTank) { return; }
            Egg.CollectedEgg -= ActivateTankWrapper;
            StartCoroutine(ActivateTank());
        }

        private IEnumerator ActivateTank()
        {
            // Step 0: Unparent from barn
            transform.parent = null;

            // Step 1: tween position in fron of barn
            transform.DOMove(transform.position + 9 * transform.forward, _awakenDuration).SetEase(Ease.InOutCubic);
            yield return new WaitForSeconds(_awakenDuration);

            // Step 2: connect to navmesh and activate
            _targeting.TargetingEnabled = true;
            _agent.enabled = true;
            _movement.enabled = true;
            _isActive = true;
            _shooting.CanShoot = true;
            _canvas.SetActive(true);
        }

        public void TakeDamage(float deltaHealth)
        {
            if (!_isActive) return;

            _currentHealth -= deltaHealth;
            _currentHealth = Mathf.Max(0, _currentHealth);
            DOTween.To(() => _healthSlider.value, x => _healthSlider.value = x, _currentHealth, 1);
            if (_currentHealth <= 0)
            {
                Destroy(gameObject);
                GameObject deathEffect = Instantiate(_deathEffect,transform.position, Quaternion.identity);
                deathEffect.transform.localScale *= 3;
                Destroy(deathEffect, 3f);
            }
        }
    }
}
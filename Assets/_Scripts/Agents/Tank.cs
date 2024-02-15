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
        [SerializeField] private float _awakenDuration = 12.8f;
        [SerializeField] private float _maxHealth;
        [SerializeField] private GameObject _canvas;
        [SerializeField] private SliderWithDelay _healthSlider;
        [SerializeField] private GameObject _deathEffect;
        [SerializeField] private Transform _turret;
        [SerializeField] private float _turretRotationSpeed;

        private bool _isActive;
        private float _currentHealth;
        private NavMeshAgent _agent;
        private TankMovement _movement;
        private Shooting _shooting;
        private Targeting _targeting;

        [SerializeField, Range(0f,1f)] float _changePhaseThreshold = 0.5f;
        private Phases _phase;
        private enum Phases
        {
            Phase1,
            Phase2
        }

        public Vector3 Position => transform.position;

        private void Awake()
        {
            _movement = GetComponent<TankMovement>();
            _movement.enabled = false;
            _targeting = GetComponent<Targeting>();
            _targeting.TargetingEnabled = false; // don't do _targeting.enabled = false else AgentMovement rotate towards target will fail
            _agent = GetComponent<NavMeshAgent>();
            _agent.enabled = false;
            _isActive = false;
            _canvas.SetActive(false);
            _currentHealth = _maxHealth;
        }

        private void Start()
        {
            _shooting = _targeting.Shooting;
            _shooting.CanShoot = false;
            _phase = Phases.Phase1;
        }

        private void OnEnable()
        {
            Egg.CollectedEgg += ActivateTankWrapper;
        }

        private void OnDisable()
        {
            Egg.CollectedEgg -= ActivateTankWrapper;
        }

        private void Update()
        {
            if (!_isActive) { return; }

            if (_targeting.IsHostile)
            {
                if (_resetTurretRotationRoutine != null)
                {
                    StopCoroutine(_resetTurretRotationRoutine);
                    _resetTurretRotationRoutine = null;
                    _turret.DOKill();
                }
                RotateTurret();
            } 
            else if (_resetTurretRotationRoutine == null && _turret.localEulerAngles.y != 0)
            {
                _resetTurretRotationRoutine = StartCoroutine(ResetTurretRotation());
            }
        }

        private Coroutine _resetTurretRotationRoutine;
        private IEnumerator ResetTurretRotation()
        {
            _turret.DOLocalRotate(Vector3.zero, 3f);
            yield return new WaitForSeconds(3f);
            _resetTurretRotationRoutine = null;
        }

        private void RotateTurret()
        {
            Vector3 direction = _targeting.Target.position - _turret.transform.position;

            // Project the direction onto the xz-plane (ignoring height)
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                _turret.transform.rotation = Quaternion.RotateTowards(_turret.transform.rotation, targetRotation, Time.deltaTime * _turretRotationSpeed);
            }
        }

        private void ActivateTankWrapper()
        {
            if (EggManager.CollectedEggs != GameParamsLoader.EggsCollectedToSpawnTank) { return; }
            Egg.CollectedEgg -= ActivateTankWrapper;
            StartCoroutine(CanvasManager.Instance.ShowNotificationRoutine("BOSS INCOMING"));
            StartCoroutine(ActivateTank());
        }

        private IEnumerator ActivateTank()
        {
            AudioManager.Instance.StopAmbiance();
            AudioManager.Instance.StartAmbiance(FMODEvents.Instance.ChickenRageHard, timedIntensity: false);
            AudioManager.Instance.UpdateMusicIntensityManually(0);

            // Step 0: Unparent from barn
            transform.parent = null;

            // Step 1: tween position in front of barn
            transform.DOMove(transform.position + 9 * transform.forward, _awakenDuration).SetEase(Ease.InOutCubic);
            yield return new WaitForSeconds(_awakenDuration);
            AudioManager.Instance.UpdateMusicIntensityManually(1);

            // Step 2: connect to navmesh and activate
            _targeting.TargetingEnabled = true;
            _agent.enabled = true;
            _movement.enabled = true;
            _isActive = true;
            _shooting.CanShoot = true;
            _canvas.SetActive(true);
            _healthSlider.Init(_maxHealth, _maxHealth);
        }

        public void TakeDamage(float deltaHealth)
        {
            if (!_isActive) return;

            _currentHealth -= deltaHealth;
            _currentHealth = Mathf.Max(0, _currentHealth);
            _healthSlider.Value = _currentHealth;

            if (_currentHealth < _changePhaseThreshold * _maxHealth && _phase == Phases.Phase1)
            {
                _phase = Phases.Phase2;
                AudioManager.Instance.UpdateMusicIntensityManually(2);
                _shooting.Cooldown *= 0.5f;
            }

            if (_currentHealth <= 0)
            {
                StartCoroutine(Die());
            }
        }

        private IEnumerator Die()
        {
            float deltaTime = 0f;
            while (deltaTime <= 3f)
            {
                deltaTime += 1f;
                GameObject deathEffect = Instantiate(_deathEffect, transform.position, Quaternion.identity);
                deathEffect.transform.localScale *= 3;
                Destroy(deathEffect, 1f);
                yield return new WaitForSeconds(1f);
            }
            AudioManager.Instance.StopAmbiance();
            AudioManager.Instance.StartAmbiance(FMODEvents.Instance.Ambiance, timedIntensity: true);
            Destroy(gameObject);
        }
    }
}
using System.Collections;
using UnityEngine;
//using MoreMountains.Feedbacks;
using StarterAssets;
using System;

namespace GGJ24
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        public static PlayerHealth Instance { get; private set; }


        public Vector3 Position { get => transform.position; private set => transform.position = value; }
        public static event Action<int> HealthChanged;
        public static event Action PlayerDeath;

        [SerializeField, Min(0)] private float _hitAnimCooldown = 2f;
        [SerializeField, Min(0)] private float _stunDuration = 1f;
        [SerializeField] private Animator _animator;
        private bool _hasAnimator;
        private bool _hasController;
        private bool _hasHitFeedback;
        private int _animIDDie;
        private int _animIDHit;
        private Coroutine _hitRoutine;

        [SerializeField] private PostProcessController _vignette;
        //[SerializeField] private MMF_Player _hitFeedback;
        private ThirdPersonController _controller;
        private bool _isInvincible;
        public bool IsCritical => Health < _criticalThreshold;
        [SerializeField, Range(0f,1f)] private float _criticalThreshold = 0.25f;
        public bool IsDead { get; private set; }
        private Coroutine _invincibilityRoutine;

        [SerializeField, Range(0f, 1f)] private float _damageReductionMultiplier = 0.3f;
        private float _currentDamageModifier = 1f;


        public float Health
        {
            get { return _health; }
            set
            {
                bool wasJustAlive = _health > 0;
                _health = Mathf.Min(MaxHealth, Mathf.Max(value, 0));

                if (_health <= 0 && wasJustAlive)
                {
                    Die();
                }
            }
        }

        [SerializeField] private float _health;
        [SerializeField, Range(0, 1000)] private float _maxHealth;
        public float MaxHealth
        {
            get { return _maxHealth; }
            set
            {
                _maxHealth = Mathf.Max(value, 0);
            }
        }

        protected virtual void Awake()
        {

            if (Instance != null)
            {
                Debug.LogWarning("Found more than one Spawner Instance");
            }
            else
            {
                Instance = this;
            }

            IsDead = false;
            _isInvincible = false;
        }

        private void Start()
        {
            _hasAnimator = _animator != null;
            if (_hasAnimator)
            {
                AssignAnimationIDs();
            }
            _hasController = TryGetComponent(out _controller);
            Debug.Log("Hard-coded initial health penalty");
            _health = _maxHealth - GameParamsLoader.GoldenEggHealAmount;
            //_hasHitFeedback = _hitFeedback != null;
        }

        private void OnEnable()
        {
            ThirdPersonController.TriggerIFRame += TriggerDamageReduction;
            ThirdPersonController.Dancing += ActivateDamageReduction;
            GameManager.GameEnded += Die;
            GoldenEgg.HealPlayer += Heal;
            AbilityManager.AbilityUnlocked += StartAutoHeal;
        }

        private void OnDisable()
        {
            ThirdPersonController.TriggerIFRame -= TriggerDamageReduction;
            ThirdPersonController.Dancing -= ActivateDamageReduction;
            GameManager.GameEnded -= Die;
            GoldenEgg.HealPlayer -= Heal;
            AbilityManager.AbilityUnlocked -= StartAutoHeal;
        }

        protected virtual void AssignAnimationIDs()
        {
            _animIDDie = Animator.StringToHash("Die");
            _animIDHit = Animator.StringToHash("GettingHit");
        }

        public virtual void TakeDamage(float deltaHealth)
        {
            if (_isInvincible || IsDead) return;
            deltaHealth *= _currentDamageModifier;
            Health -= deltaHealth;
            HealthChanged?.Invoke((int)Health);
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.HitSFX, transform.position);
            if (Health > 0)
            {
                _hitRoutine ??= StartCoroutine(HitRoutine());
            }
            UpdateCriticalVignette();
        }

        public void ActivateDamageReduction(bool activate)
        {
            _currentDamageModifier = activate ? _damageReductionMultiplier : 1f;
        }

        public void TriggerDamageReduction(float duration)
        {
            if (_invincibilityRoutine != null) return;
            _invincibilityRoutine = StartCoroutine(InvincibilityRoutine(duration));
        }

        private IEnumerator InvincibilityRoutine(float duration)
        {
            _isInvincible = true;
            yield return new WaitForSeconds(duration);
            _isInvincible = false;
            _invincibilityRoutine = null;
        }

        protected virtual IEnumerator HitRoutine()
        {
            //if (_hasHitFeedback) _hitFeedback.PlayFeedbacks();
            //if (_hasController) _controller.Stun(_stunDuration);
            if (_hasAnimator && !ThirdPersonController.IsDancing) _animator.SetTrigger(_animIDHit);
            yield return new WaitForSeconds(_hitAnimCooldown);
            _hitRoutine = null;
        }

        private void StartAutoHeal(AbilityManager.Ability ability)
        {
            if (ability.Name != AbilityManager.AutoHeal.Name) { return; }
            AbilityManager.AbilityUnlocked -= StartAutoHeal;
            StartCoroutine(AutoHealRoutine());
        }

        private IEnumerator AutoHealRoutine()
        {
            while(true)
            {
                Heal(GameParamsLoader.AutoHealPerSecond);
                yield return new WaitForSeconds(1f);
            }
        }

        public void Heal(float deltaHealth)
        {
            Health += deltaHealth;
            HealthChanged?.Invoke((int)Health);
            UpdateCriticalVignette();
        }

        private void UpdateCriticalVignette()
        {
            if (Health / MaxHealth > _criticalThreshold) _vignette.SetVignetteIntensity(0f);
            else _vignette.SetVignetteIntensity(Mathf.Lerp(0f, 0.5f, 1 - Health / MaxHealth));
        }

        [SerializeField] private GameObject _deathEffect;
        protected virtual void Die()
        {
            if (IsDead) return;
            PlayerDeath?.Invoke();
            IsDead = true;
            _animator.SetTrigger(_animIDDie);
            GameManager.Instance.EndGame();
            StartCoroutine(DeathRoutine());
        }
        private IEnumerator DeathRoutine()
        {
            yield return new WaitForSeconds(3f);
            Instantiate(_deathEffect, transform.position + 0.3f*Vector3.up, Quaternion.identity);
        }
    }
}
using System.Collections;
using UnityEngine;
//using MoreMountains.Feedbacks;
using StarterAssets;
using System;

namespace GGJ24
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
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
        private bool _isInvincible = false;
        public static bool IsDead { get; private set; }
        private Coroutine _invincibilityRoutine;

        public float Health
        {
            get { return _health; }
            set
            {
                bool wasJustAlive = IsAlive;
                _health = Mathf.Min(InitialHealth, Mathf.Max(value, 0));

                if (_health <= 0 && wasJustAlive)
                {
                    Die();
                }
            }
        }

        public bool IsAlive
        {
            get { return _health > 0; }
        }

        [SerializeField] private float _health;
        [SerializeField, Range(0, 1000)] private float _initialHealth;
        public float InitialHealth
        {
            get { return _initialHealth; }
            set
            {
                _initialHealth = Mathf.Max(value, 0);
            }
        }

        protected virtual void Awake()
        {
            IsDead = false;
        }

        private void Start()
        {
            _hasAnimator = _animator != null;
            if (_hasAnimator)
            {
                AssignAnimationIDs();
            }
            _hasController = TryGetComponent(out _controller);
            _health = _initialHealth - GameParamsLoader.GoldenEggHealAmount;
            //_hasHitFeedback = _hitFeedback != null;
        }

        private void OnEnable()
        {
            ThirdPersonController.TriggerIFRame += TriggerInvincibility;
            GameManager.GameEnded += Die;
            GoldenEgg.HealPlayer += Heal;
        }

        private void OnDisable()
        {
            ThirdPersonController.TriggerIFRame -= TriggerInvincibility;
            GameManager.GameEnded -= Die;
            GoldenEgg.HealPlayer -= Heal;
        }

        protected virtual void AssignAnimationIDs()
        {
            _animIDDie = Animator.StringToHash("Die");
            _animIDHit = Animator.StringToHash("GettingHit");
        }

        public virtual void TakeDamage(float deltaHealth)
        {
            if (_isInvincible || IsDead) return;

            Health -= deltaHealth;
            HealthChanged?.Invoke((int)Health);
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.HitSFX, transform.position);
            if (Health > 0)
            {
                _hitRoutine ??= StartCoroutine(HitRoutine());
                if (Health / InitialHealth > 0.33f) _vignette.SetVignetteIntensity(0f);
                else _vignette.SetVignetteIntensity(Mathf.Lerp(0f, 0.5f, 1 - Health / InitialHealth));
            }
        }

        public void TriggerInvincibility(float duration)
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

        public void Heal(float deltaHealth)
        {
            Health += deltaHealth;
            HealthChanged?.Invoke((int)Health);
        }

        protected virtual void Die()
        {
            if (IsDead) return;
            PlayerDeath?.Invoke();
            IsDead = true;
            _animator.SetTrigger(_animIDDie);
            GameManager.Instance.EndGame();
        }
    }
}
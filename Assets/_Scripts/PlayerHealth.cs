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
        public static event Action<int> TookDamage;

        [SerializeField] private float _health;
        [SerializeField, Min(0)] private float _hitAnimCooldown = 2f;
        [SerializeField, Min(0)] private float _stunDuration = 1f;
        private Animator _animator;
        private bool _hasAnimator;
        private bool _hasController;
        private bool _hasHitFeedback;
        private int _animIDHit;
        private Coroutine _hitRoutine;

        //[SerializeField] private MMF_Player _hitFeedback;
        private ThirdPersonController _controller;

        public float Health
        {
            get { return _health; }
            set
            {
                bool wasJustAlive = IsAlive;
                _health = Mathf.Max(value, 0);

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
            _health = _initialHealth;
        }

        private void Start()
        {
            _hasAnimator = TryGetComponent(out _animator);
            if (_hasAnimator)
            {
                AssignAnimationIDs();
            }
            _hasController = TryGetComponent(out _controller);
            //_hasHitFeedback = _hitFeedback != null;
        }

        protected virtual void AssignAnimationIDs()
        {
            _animIDHit = Animator.StringToHash("Hit");
        }

        public virtual void TakeDamage(float deltaHealth)
        {
            Health -= deltaHealth;
            TookDamage?.Invoke((int)Health);
            Debug.Log("took damage" + Health);
            //_hitRoutine ??= StartCoroutine(HitRoutine());
        }

        protected virtual IEnumerator HitRoutine()
        {
            //if (_hasHitFeedback) _hitFeedback.PlayFeedbacks();
            //if (_hasController) _controller.Stun(_stunDuration);
            if (_hasAnimator) _animator.SetTrigger(_animIDHit);
            yield return new WaitForSeconds(_hitAnimCooldown);
            _hitRoutine = null;
        }

        public void Heal(float deltaHealth)
        {
            Health += deltaHealth;
        }

        protected virtual void Die()
        {
            Debug.Log("Entity has died.");
            GameManager.Instance.GameOver();
        }
    }
}
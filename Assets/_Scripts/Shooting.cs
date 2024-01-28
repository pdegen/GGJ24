using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ24
{
    public class Shooting : MonoBehaviour
    {
        public bool IsHostile { get; set; }
        public bool CanShoot = true;

        [SerializeField] protected int _damageAmount = 1;
        [SerializeField] protected float _weaponRange = 100f;
        [SerializeField] protected float _bulletSpeed = 200f;
        [SerializeField] protected ParticleSystem _shootingSystem;
        [SerializeField] protected ParticleSystem _impactSystem;
        [SerializeField] private Chicken _chicken;

        public bool IsShooting { get; private set; }
        [SerializeField] private float _cooldown = 2f;

        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private float _randomShootPeriod = 5f;
        [SerializeField, Range(0f, 1f)] private float _randomShootProbability = 0.1f;

        protected Vector3 _firedirection;
        private float _cooldownDeltaTime = 0f;

        private void Awake()
        {
            Gizmos.color = Color.magenta;
        }

        private void Start()
        {
            InvokeRepeating(nameof(RandomShoot), 2.0f, _randomShootPeriod);
            _chicken = GetComponentInParent<Chicken>();
        }

        private void OnEnable()
        {
            Egg.CollectedEgg += OnEggCollected;
        }

        private void OnDisable()
        {
            Egg.CollectedEgg -= OnEggCollected;
        }

        private void OnEggCollected()
        {
            _randomShootProbability *= GameManager.Instance.RageMultiplier;
            _randomShootPeriod = Mathf.Max(1, _randomShootPeriod /= GameManager.Instance.RageMultiplier);
        }

        private void RandomShoot()
        {
            if (!CanShoot) return;
            if (Random.Range(0f, 1f) < _randomShootProbability) Shoot();
        }

        protected virtual void Shoot()
        {
            GameObject bullet = Instantiate(_bulletPrefab, transform.position, transform.rotation);
            bullet.GetComponent<Rigidbody>().velocity = _bulletSpeed * transform.forward;
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ShootSFX, transform.position);
        }

        private void Update()
        {
            if (!IsHostile)
            {
                return;
            }

            if (_cooldownDeltaTime > _cooldown && CanShoot && !IsShooting)
            {
                Shoot();
                _chicken.Recoil();
                _cooldownDeltaTime = 0;
            }
            _cooldownDeltaTime += Time.deltaTime;
        }

        public void CommenceHostilities()
        {
            if (IsHostile) return;
            IsHostile = true;
            _cooldownDeltaTime = 0.1f * _cooldown;
        }

        public void StopShooting()
        {

        }
    }
}
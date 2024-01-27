using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ24
{
    public class Shooting : MonoBehaviour
    {

        [SerializeField] protected int _damageAmount = 1;
        [SerializeField] protected float _weaponRange = 100f;
        [SerializeField] protected float _bulletSpeed = 200f;

        [SerializeField] protected ParticleSystem _shootingSystem;
        [SerializeField] protected ParticleSystem _impactSystem;

        [SerializeField] protected Transform _firepoint;
        protected Vector3 _firedirection;
        protected bool _isShooting = false;
        public bool IsShooting { get => _isShooting; private set => _isShooting = value; }
        [SerializeField] private float _cooldown = 2f;

        [SerializeField] private GameObject _bulletPrefab;

        private float _cooldownDeltaTime = 0f;
        private bool _isHostile = false;
        public bool IsHostile { get => _isHostile; set => _isHostile = value; }
        public bool CanShoot = true;

        [SerializeField] private float _randomShootPeriod = 5f;
        [SerializeField, Range(0f,1f)] private float _randomShootProbability = 0.1f;


        private void Awake()
        {
            Gizmos.color = Color.magenta;

            if (_firepoint == null)
            {
                _firepoint = transform;
            }
        }

        private void Start()
        {
            InvokeRepeating(nameof(RandomShoot), 2.0f, _randomShootPeriod);
        }

        private void RandomShoot()
        {
            if (!CanShoot) return;
            if (Random.Range(0f, 1f) < _randomShootProbability) Shoot();
        }

        protected virtual void Shoot()
        {
            GameObject bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().velocity = _bulletSpeed * transform.forward;
        }

        private void Update()
        {
            if (!IsHostile)
            {
                return;
            }

            if (_cooldownDeltaTime > _cooldown && CanShoot && !_isShooting)
            {
                Shoot();
                _cooldownDeltaTime = 0;
            }
            _cooldownDeltaTime += Time.deltaTime;
            _firedirection = _firepoint.forward; // shitty solution
        }

        public void CommenceHostilities()
        {
            if (IsHostile) return;
            IsHostile = true;
            _cooldownDeltaTime = 0.5f * _cooldown;
        }

        public void StopShooting()
        {

        }
    }
}
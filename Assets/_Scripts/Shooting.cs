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
        [SerializeField] private float _fireRate = 0.15f;
        [SerializeField] private float _cooldown = 2f;
        [SerializeField] private float _commenceHostilitiesDelay = 1.6f;
        [SerializeField] private int _shotsPerBurst = 15;

        [SerializeField] private GameObject _bulletPrefab;

        private float _cooldownDeltaTime = 0f;
        private bool _isHostile = false;
        public bool IsHostile { get => _isHostile; set => _isHostile = value; }
        public bool CanShoot = true;


        private void Awake()
        {
            Gizmos.color = Color.magenta;

            if (_firepoint == null)
            {
                _firepoint = transform;
            }
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
                StartCoroutine(BurstFire());
            }
            _cooldownDeltaTime += Time.deltaTime;
            _firedirection = _firepoint.forward; // shitty solution
        }

        public IEnumerator CommenceHostilities()
        {
            yield return new WaitForSeconds(_commenceHostilitiesDelay);
            IsHostile = true;
            _cooldownDeltaTime = _cooldown;
        }

        private IEnumerator BurstFire()
        {
            _isShooting = true;
            // TO DO: Object pooling
            for (int i = 0; i < _shotsPerBurst; ++i)
            {
                if (!IsHostile) break;
                Shoot();
                yield return new WaitForSeconds(_fireRate);
            }
            _isShooting = false;
            _cooldownDeltaTime = 0f;
        }
    }
}
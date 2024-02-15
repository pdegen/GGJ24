using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ24
{
    public class Shooting : MonoBehaviour
    {
        public bool IsHostile { get; set; } = false;
        public bool CanShoot { get; set; } = true;

        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private float _bulletSpeed = 10f;
        public float Cooldown = 2f;
        [SerializeField] private float _randomShootPeriod = 5f;
        [SerializeField, Range(0f, 1f)] private float _randomShootProbability = 0.1f;
        [SerializeField] private Transform _firepoint;

        private Vector3 _firedirection;
        private float _cooldownDeltaTime = 0f;
        private AgentMovement _movement;

        private void Start()
        {
            InvokeRepeating(nameof(RandomShoot), 2.0f, _randomShootPeriod);
            _movement = GetComponentInParent<AgentMovement>();
        }

        private void OnEnable()
        {
            Egg.CollectedEgg += OnEggCollected;
        }

        private void OnDisable()
        {
            Egg.CollectedEgg -= OnEggCollected;
        }

        private void Update()
        {
            if (!IsHostile)
            {
                return;
            }

            if (_cooldownDeltaTime > Cooldown && CanShoot)
            {
                Shoot();
                _movement.Recoil();
                _cooldownDeltaTime = 0;
            }
            _cooldownDeltaTime += Time.deltaTime;
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
            GameObject bullet = Instantiate(_bulletPrefab, _firepoint.position, _firepoint.rotation);
            bullet.GetComponent<Rigidbody>().velocity = _bulletSpeed * _firepoint.forward;
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ShootSFX, _firepoint.position);
        }

        public void CommenceHostilities()
        {
            if (IsHostile) return;
            IsHostile = true;
            _cooldownDeltaTime = 0;
        }
    }
}
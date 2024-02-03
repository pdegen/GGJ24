using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;
using System;

namespace GGJ24
{
    public class Missile : MonoBehaviour
    {
        public static float ExplosionRadius = 5f;

        [SerializeField] private GameObject _explosionEffect;
        [SerializeField] private GameObject _explosionDecal;
        [SerializeField] private float _explosionForce = 10f;
        [SerializeField] private float _explosionBaseDamage;
        [SerializeField] private float _upwardsModifier = 3f; // Adjust the force applied upwards
        [SerializeField] private float _explosionDamage = 50f;
        [SerializeField, Range(0f, 1f)] private float _gravity = 0.01f;
        [SerializeField] private Transform _decalSpawnPoint;
        [SerializeField] private DamageNumber _dodgeNumberPrefab;
        // TO DO: Limit max decals?

        // Check if missile hits default layer to rotate decal or water layer to splash
        private readonly float _raycastDistance = 1.5f;

        private void Start ()
        {
            Destroy(gameObject, 20f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 12) return; // magnet

            if (other.gameObject.layer == 6 && UnityEngine.Random.Range(0f, 1f) < ThirdPersonController.DodgeProbability)
            {
                _dodgeNumberPrefab.Spawn(0.4f*other.transform.up + other.transform.position, "Dodged!");
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.WhooshSFX, transform.position);
                return;
            }

            Explode();
            Destroy(gameObject);
        }

        private void Update()
        {
            transform.position -= _gravity * Time.deltaTime * Vector3.up;
        }

        private void Explode()
        {
            Instantiate(_explosionEffect, transform.position, Quaternion.identity);
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ExplosionSFX, transform.position);

            // Water
            if (Physics.Raycast(transform.position, -Vector3.up, out RaycastHit waterHit, _raycastDistance, layerMask: 1 << 4))
            {
                if (waterHit.collider.TryGetComponent(out WaterEffects waterEffects))
                {
                    waterEffects.SpawnBigSplash(transform.position);
                }
            }

            // Decal
            if (Physics.Raycast(_decalSpawnPoint.position, _decalSpawnPoint.transform.forward, out RaycastHit decalhit, _raycastDistance, layerMask: 1 << 0))
            {
                GameObject decal = Instantiate(_explosionDecal, _decalSpawnPoint.position, Quaternion.identity);
                decal.transform.forward = -decalhit.normal;
            }
            else
            {
                Instantiate(_explosionDecal, transform.position, Quaternion.Euler(90, 0, 0));
            }

            Collider[] colliders = Physics.OverlapSphere(transform.position, ExplosionRadius);

            foreach (Collider hit in colliders)
            {
                if (hit.TryGetComponent(out Chicken chicken))
                {
                    if (chicken.IsSleeping) { chicken.WakeUpWrapper(); }

                    chicken.TemporarilyDisableNavMeshAgentWrapper();

                    if (hit.TryGetComponent(out Rigidbody rb))
                    {
                        rb.AddExplosionForce(_explosionForce, transform.position, ExplosionRadius, _upwardsModifier, ForceMode.Impulse);
                    }
                }

                if (hit.TryGetComponent(out PlayerHealth playerHealth))
                {
                    playerHealth.TakeDamage(_explosionDamage);
                }
            }

            Destroy(gameObject);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, -Vector3.up * _raycastDistance);
            Gizmos.color = Color.black;
            Gizmos.DrawRay(_decalSpawnPoint.position, _decalSpawnPoint.transform.forward * _raycastDistance);
        }
    }
}
using GGJ24;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public static float ExplosionRadius = 5f;

    [SerializeField] private GameObject _explosionEffect;
    [SerializeField] private float _explosionForce = 10f;
    [SerializeField] private float _explosionBaseDamage;
    [SerializeField] private float _upwardsModifier = 3f; // Adjust the force applied upwards
    [SerializeField] private float _explosionDamage = 50f;
    [SerializeField,Range(0f,1f)] private float _gravity = 0.01f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 12) return; // magnet
        Explode();
        Destroy(gameObject);
    }

    private void Update()
    {
        transform.position -= Vector3.up * _gravity * Time.deltaTime;
    }

    private void Explode()
    {
        Instantiate(_explosionEffect, transform.position, Quaternion.identity);
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ExplosionSFX, transform.position);

        Collider[] colliders = Physics.OverlapSphere(transform.position, ExplosionRadius);

        foreach (Collider hit in colliders)
        {
            if (hit.TryGetComponent(out Chicken chicken))
            {
                if (chicken.IsSleeping) { chicken.WakeUp(); }

                chicken.CoroutineWrapper();

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
}

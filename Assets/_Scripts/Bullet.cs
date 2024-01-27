using GGJ24;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject _explosionEffect;
    public static float ExplosionRadius = 5f;
    [SerializeField] private float _explosionForce = 10f;
    [SerializeField] private float _explosionBaseDamage;
    [SerializeField] private float _upwardsModifier = 3f; // Adjust the force applied upwards
    private void OnTriggerEnter(Collider other)
    {
        Explode();
        Destroy(gameObject);
    }

    private void Explode()
    {
        Instantiate(_explosionEffect, transform.position, Quaternion.identity);

        // Get all colliders within the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, ExplosionRadius);

        foreach (Collider hit in colliders)
        {
            if (hit.TryGetComponent(out Chicken chicken))
            {
                if (chicken.IsSleeping) { chicken.WakeUp(); }

                chicken.CoroutineWrapper();

                // Check if the object has a Rigidbody
                if (hit.TryGetComponent(out Rigidbody rb))
                {
                    //Debug.Log("apply force");
                    // Apply explosion force to the Rigidbody
                    rb.AddExplosionForce(_explosionForce, transform.position, ExplosionRadius, _upwardsModifier, ForceMode.Impulse);
                }
            }
        }

        // Optional: Destroy the object after the explosion
        Destroy(gameObject);
    }
}

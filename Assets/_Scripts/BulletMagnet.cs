using GGJ24;
using UnityEngine;

public class BulletMagnet : MonoBehaviour
{
    [SerializeField] private float _magnetStrength = 10f;
    [SerializeField] private float _attractRadius = 5f;
    public LayerMask _attractLayer = 1 << 9;

    private void OnEnable()
    {
        Egg.CollectedEgg += OnEggCollected;
    }

    private void OnDisable()
    {
        Egg.CollectedEgg -= OnEggCollected;
    }

    void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _attractRadius, _attractLayer);
        foreach (Collider col in colliders)
        {
            if (col.TryGetComponent<Rigidbody>(out var rb))
            {
                Attract(rb);
            }
        }
    }

    void Attract(Rigidbody rb)
    {
        Vector3 direction = transform.position - rb.position;
        float distance = direction.magnitude;

        if (distance == 0f)
        {
            return; // Prevent division by zero
        }

        float forceMagnitude = _magnetStrength * (1f / distance);
        Vector3 force = direction.normalized * forceMagnitude;

        rb.AddForce(force);
    }

    private void OnEggCollected()
    {
        _magnetStrength *= GameManager.Instance.RageMultiplier;
        _attractRadius *= GameManager.Instance.RageMultiplier;
    }
}

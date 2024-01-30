using DG.Tweening;
using GGJ24;
using StarterAssets;
using UnityEngine;

public class BulletMagnet : MonoBehaviour
{
    [SerializeField] private float _attractStrength = 10f;
    [SerializeField] private float _repelStrength = 1f;
    [SerializeField] private float _attractRadius = 5f;
    public LayerMask _attractLayer = 1 << 9;
    private bool _isRepelling = false;

    private void OnEnable()
    {
        Egg.CollectedEgg += OnEggCollected;
        ThirdPersonController.Dancing += TogglePolarity;
    }

    private void OnDisable()
    {
        Egg.CollectedEgg -= OnEggCollected;
        ThirdPersonController.Dancing -= TogglePolarity;
    }

    void FixedUpdate()
    {
        if (_isRepelling) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, _attractRadius, _attractLayer);
        foreach (Collider col in colliders)
        {
            if (col.TryGetComponent<Rigidbody>(out var rb))
            {
                if (_isRepelling) Repel(rb);
                else Attract(rb);
            }
        }
    }

    private void TogglePolarity(bool isRepelling)
    {
        _isRepelling = isRepelling;
    }
    void Attract(Rigidbody rb)
    {
        Vector3 direction = transform.position - rb.position;
        float distance = direction.magnitude;

        if (distance == 0f)
        {
            return; // Prevent division by zero
        }

        float forceMagnitude = _attractStrength * (1f / distance);
        Vector3 force = direction.normalized * forceMagnitude;

        rb.AddForce(force);
        rb.transform.rotation = Quaternion.LookRotation(rb.velocity.normalized);
    }
    void Repel(Rigidbody rb)
    {
        Vector3 direction = transform.position - rb.position;
        float distance = Mathf.Min(1, direction.magnitude);

        float forceMagnitude = _repelStrength * (1f / distance);
        Vector3 force = direction.normalized * forceMagnitude;

        rb.AddForce(-force);
        rb.transform.rotation = Quaternion.LookRotation(rb.velocity.normalized);
    }
    private void OnEggCollected()
    {
        _attractStrength *= GameManager.Instance.RageMultiplier;
        _attractRadius *= GameManager.Instance.RageMultiplier;
    }
}

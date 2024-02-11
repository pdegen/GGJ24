using DG.Tweening;
using StarterAssets;
using UnityEngine;

namespace GGJ24
{
    public class BulletMagnet : MonoBehaviour
    {
        [SerializeField] private float _attractStrength = 10f;
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
            if (AbilityManager.Reflect.IsUnlocked && ThirdPersonController.IsDancing) { return; }

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

            float forceMagnitude = _attractStrength * (1f / distance);
            Vector3 force = direction.normalized * forceMagnitude;

            rb.AddForce(force);
            rb.transform.rotation = Quaternion.LookRotation(rb.velocity.normalized);
        }
        private void OnEggCollected()
        {
            _attractStrength *= GameManager.Instance.RageMultiplier;
            _attractRadius *= GameManager.Instance.RageMultiplier;
        }
    }
}
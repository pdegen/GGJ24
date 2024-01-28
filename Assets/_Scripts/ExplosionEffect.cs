using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ24
{
    public class ExplosionEffect : MonoBehaviour
    {
        [SerializeField] private float _effectDuration = 0.2f;
        private float _timer = 0;

        void Update()
        {
            _timer += Time.deltaTime;
            if (_timer > _effectDuration)
                Destroy(gameObject);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, Bullet.ExplosionRadius);
        }
    }
}
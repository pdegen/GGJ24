using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        // Set the color of the gizmo
        Gizmos.color = Color.red;

        // Draw a wire sphere at the position of the explosion with the specified radius
        Gizmos.DrawWireSphere(transform.position, Bullet.ExplosionRadius);
    }
}

using UnityEngine;

namespace GGJ24
{
    public interface IDamageable
    {
        Vector3 Position { get; } // for distance-based damage like explosions
        void TakeDamage(float deltaHealth);
    }
}
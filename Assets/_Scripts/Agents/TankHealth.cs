using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ24
{
    public class TankHealth : MonoBehaviour, IDamageable
    {
        public Vector3 Position => transform.position;
        private Tank _tank;

        private void Awake()
        {
            _tank = GetComponentInParent<Tank>();
        }

        public void TakeDamage(float deltaHealth)
        {
            _tank.TakeDamage(deltaHealth);
        }
    }
}
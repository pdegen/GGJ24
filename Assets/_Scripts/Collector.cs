using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ24
{
    public class Collector : MonoBehaviour
    {
        private ICollecetable _collectable;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out ICollecetable collectable))
            {
                _collectable = collectable;
                _collectable.Collect(this);
            }
            else
            {
                Debug.LogWarning("ICollectable not implemented for collider " + other.name);
            }
        }
    }
}
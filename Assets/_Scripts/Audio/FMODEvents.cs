using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

namespace GGJ24
{
    public sealed class FMODEvents : MonoBehaviour
    {
        [field: Header("Ambiance")]
        [field: SerializeField] public EventReference Ambiance { get; private set; }

        [field: Header("Explosion SFX")]
        [field: SerializeField] public EventReference ExplosionSFX { get; private set; }

        public static FMODEvents Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Debug.LogWarning("Found more than one FMODEvents Instance!");
            }
            Instance = this;
        }
    }
}
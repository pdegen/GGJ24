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

        [field: Header("Shoot SFX")]
        [field: SerializeField] public EventReference ShootSFX { get; private set; }
        
        [field: Header("Collection SFX")]
        [field: SerializeField] public EventReference CollectionSFX { get; private set; }
        [field: Header("Hit SFX")]
        [field: SerializeField] public EventReference HitSFX { get; private set; }
        [field: Header("Victory SFX")]
        [field: SerializeField] public EventReference VictorySFX { get; private set; }
        [field: Header("Game Over SFX")]
        [field: SerializeField] public EventReference GameOverSFX { get; private set; }

        [field: Header("Chicken Mood SFX")]
        [field: SerializeField] public EventReference ChickenMoodSFX { get; private set; }
        [field: Header("Player Step SFX")]
        [field: SerializeField] public EventReference PlayerStepFX { get; private set; }
        [field: Header("Player Water Step SFX")]
        [field: SerializeField] public EventReference PlayerWaterStepFX { get; private set; }
        [field: Header("Player Land SFX")]
        [field: SerializeField] public EventReference PlayerLandSFX { get; private set; }
        [field: Header("Whoosh SFX")]
        [field: SerializeField] public EventReference WhooshSFX { get; private set; }
        [field: Header("Splash SFX")]
        [field: SerializeField] public EventReference SplashSFX { get; private set; }
        public static FMODEvents Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Found more than one FMODEvents Instance!");
            }
            Instance = this;
        }
    }
}
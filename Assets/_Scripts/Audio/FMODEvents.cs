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
        [field: Header("ChickenRageHard")]
        [field: SerializeField] public EventReference ChickenRageHard { get; private set; }

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
        [field: Header("PunchImpact SFX")]
        [field: SerializeField] public EventReference PunchImpactSFX { get; private set; }
        [field: Header("Splash SFX")]
        [field: SerializeField] public EventReference SplashSFX { get; private set; }
        [field: Header("Chicken Normal SFX")]
        [field: SerializeField] public EventReference ChickenNormal { get; private set; }
        [field: Header("Chicken Angry SFX")]
        [field: SerializeField] public EventReference ChickenAngrySFX { get; private set; }
        [field: Header("Chicken Wrathful SFX")]
        [field: SerializeField] public EventReference ChickenWrathfulSFX { get; private set; }
        [field: Header("UI Select SFX")]
        [field: SerializeField] public EventReference UISelectSFX { get; private set; }
        [field: Header("UI Submit SFX")]
        [field: SerializeField] public EventReference UISubmtiSFX { get; private set; }
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
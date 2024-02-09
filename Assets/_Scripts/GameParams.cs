using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ24
{
    [CreateAssetMenu(fileName = "GameParams", menuName = "ScriptableObject/GameParams")]
    public class GameParams : ScriptableObject
    {
        [Tooltip("Available time at the start of the game")]
        public float StartTime;

        [Header("Egg Params")]
        public float BasicEggTimeBonus;

        [Range(0f, 1f)] public float MovingEggSpawnChance;
        [Range(0f, 1f)] public float JumpingEggSpawnChance;
        [Range(0f, 1f)] public float GoldenEggSpawnChance;

        public int MovingEggSpawnMinEggCollected;
        public int JumpingEggSpawnMinEggCollected;
        public int GoldenEggSpawnMinEggCollected;

        public float MovingEggSpeed;

        public float JumpingEggAdditiveTimeBonus = 2f;
        public float MovingEggAdditiveTimeBonus = 1f;
        public float GoldenEggAdditiveTimeBonus = 5f;

        public float GoldenEggHealAmount = 200f;
        public float GoldenEggLifetime = 10f;

        [Header("Difficulty Settings")]
        [Range(0.1f,1f)] public float EasyMultiplier = 0.5f;
        [Range(1f,3f)] public float HardMultiplier = 1.5f;

        [Header("Misc.")]

        [Tooltip("How many eggs to collect before unlocking new ability")]
        public int EggsCollectedToUnlockDoubleJump;
        public int EggsCollectedToUnlockDash;
        public int EggsCollectedToUnlockDodge;
        public int EggsCollectedToUnlockReflectMissiles;
        public int EggsCollectedToSpawnTank;

        [Tooltip("How long chickens are incapacitated after firing missile")]
        public float ChickenKnockoutDuration;

        public float MissileDamage;

        [Tooltip("Dodge missile probability while dancing")]
        [Range(0f, 1f)] public float DodgeChance;
    }
}
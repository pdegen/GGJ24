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

        public float GoldenEggTimeBonus = 10f;
        public float GoldenEggHealAmount = 200f;
        public float GoldenEggLifetime = 10f;

        [Header("Misc.")]

        [Tooltip("How many eggs to collect before unlocking new ability")]
        public int EggsCollectedToUnlockDoubleJump;
        public int EggsCollectedToUnlockDash;
        public int EggsCollectedToUnlockDodge;

        [Tooltip("Dodge missile probability while dancing")]
        [Range(0f, 1f)] public float DodgeChance;
    }
}
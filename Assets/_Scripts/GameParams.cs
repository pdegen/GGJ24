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

        [Header("Egg Time Bonuses")]
        public float BasicEggTimeBonus;

        [Header("Misc.")]

        [Tooltip("How many eggs to collect before unlocking dance/dodge ability")]
        public int EggsCollectedToUnlockDodge;

        [Tooltip("Dodge missile probability while dancing")]
        [Range(0f, 1f)] public float DodgeChance;
    }
}
using FMOD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ24
{
    public class GameParamsLoader : MonoBehaviour
    {

        [SerializeField] private GameParams _params;

        public static float StartTime { get; private set; }
        public static float BasicEggTimeBonus { get; private set; }
        public static float MovingEggSpawnChance { get; private set; }
        public static float JumpingEggSpawnChance { get; private set; }
        public static float GoldenEggSpawnChance { get; private set; }
        public static float MovingEggSpawnMinEggCollected { get; private set; }
        public static float JumpingEggSpawnMinEggCollected { get; private set; }
        public static float GoldenEggSpawnMinEggCollected { get; private set; }
        public static float MovingEggSpeed { get; private set; }
        public static float GoldenEggTimeBonus { get; private set; }
        public static float GoldenEggLifetime { get; private set; }
        public static float GoldenEggHealAmount { get; private set; }
        public static float DodgeChance { get; private set; }
        public static int EggsCollectedToUnlockDodge { get; private set; }
        public static int EggsCollectedToUnlockDash { get; private set; }
        public static int EggsCollectedToUnlockDoubleJump { get; private set; }
        public static float EasyMultiplier { get; private set; }
        public static float HardMultiplier { get; private set; }
        public static float WaterLevel { get; private set; }
        public static float ChickenKnockoutDuration { get; private set; }
        public static float MissileDamage { get; private set; }

        [SerializeField] private Transform _waterObject;

        void Awake()
        {
            // EGGS
            BasicEggTimeBonus = _params.BasicEggTimeBonus;

            MovingEggSpawnChance = _params.MovingEggSpawnChance;
            JumpingEggSpawnChance = _params.JumpingEggSpawnChance;
            GoldenEggSpawnChance = _params.GoldenEggSpawnChance;

            MovingEggSpawnMinEggCollected = _params.MovingEggSpawnMinEggCollected;
            JumpingEggSpawnMinEggCollected = _params.JumpingEggSpawnMinEggCollected;
            GoldenEggSpawnMinEggCollected = _params.GoldenEggSpawnMinEggCollected;

            MovingEggSpeed = _params.MovingEggSpeed;

            GoldenEggTimeBonus = _params.GoldenEggTimeBonus;
            GoldenEggLifetime = _params.GoldenEggLifetime;
            GoldenEggHealAmount = _params.GoldenEggHealAmount;

            // DIFFICULTY
            EasyMultiplier = _params.EasyMultiplier;
            HardMultiplier = _params.HardMultiplier;

            // MISC
            StartTime = _params.StartTime;
            DodgeChance = _params.DodgeChance;
            ChickenKnockoutDuration = _params.ChickenKnockoutDuration;
            MissileDamage = _params.MissileDamage;
            EggsCollectedToUnlockDodge = _params.EggsCollectedToUnlockDodge;
            EggsCollectedToUnlockDoubleJump = _params.EggsCollectedToUnlockDoubleJump;
            EggsCollectedToUnlockDash = _params.EggsCollectedToUnlockDash;
        }

        private void Start()
        {
            WaterLevel = _waterObject.transform.position.y;
        }

        public static void AdjustDifficulty(GameManager.Difficulty difficulty)
        {
            float multiplier = 1f;

            switch(difficulty)
            {
                case GameManager.Difficulty.EASY:
                    multiplier = EasyMultiplier; break;
                case GameManager.Difficulty.HARD:
                    multiplier = HardMultiplier; break;
            }

            GoldenEggHealAmount /= multiplier;
            GoldenEggLifetime /= multiplier;
            GoldenEggTimeBonus /= multiplier;
            BasicEggTimeBonus /= multiplier;
            ChickenKnockoutDuration /= multiplier;
            MissileDamage *= multiplier;
            MovingEggSpeed *= multiplier;
        }

    }
}
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
        public static float GoldenEggTimeBonus { get; private set; }
        public static float GoldenEggLifetime { get; private set; }
        public static float GoldenEggHealAmount { get; private set; }
        public static float DodgeChance { get; private set; }
        public static int EggsCollectedToUnlockDodge { get; private set; }
        public static int EggsCollectedToUnlockDash { get; private set; }
        public static int EggsCollectedToUnlockDoubleJump { get; private set; }
        public static float WaterLevel { get; private set; }
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

            GoldenEggTimeBonus = _params.GoldenEggTimeBonus;
            GoldenEggLifetime = _params.GoldenEggLifetime;
            GoldenEggHealAmount = _params.GoldenEggHealAmount;

            // MISC
            StartTime = _params.StartTime;
            DodgeChance = _params.DodgeChance;
            EggsCollectedToUnlockDodge = _params.EggsCollectedToUnlockDodge;
            EggsCollectedToUnlockDoubleJump = _params.EggsCollectedToUnlockDoubleJump;
            EggsCollectedToUnlockDash = _params.EggsCollectedToUnlockDash;
        }

        private void Start()
        {
            WaterLevel = _waterObject.transform.position.y;
        }

    }
}
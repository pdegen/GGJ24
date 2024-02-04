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
        public static float DodgeChance { get; private set; }
        public static int EggsCollectedToUnlockDodge { get; private set; }
        public static float WaterLevel { get; private set; }
        [SerializeField] private Transform _waterObject;

        void Awake()
        {
            StartTime = _params.StartTime;
            BasicEggTimeBonus = _params.BasicEggTimeBonus;
            DodgeChance = _params.DodgeChance;
            EggsCollectedToUnlockDodge = _params.EggsCollectedToUnlockDodge;
        }

        private void Start()
        {
            WaterLevel = _waterObject.transform.position.y;
        }

    }
}
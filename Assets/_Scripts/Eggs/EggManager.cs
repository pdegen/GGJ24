using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace GGJ24
{
    public class EggManager : MonoBehaviour
    {
        public static EggManager Instance { get; private set; }

        public static Action DodgeUnlocked;
        public static int CollectedEggs { get; set; }

        [SerializeField] private float _eggSpawnRadius = 40f;
        [SerializeField] private float _yoffset = 0.5f;
        [SerializeField] private GameObject _eggPrefab;

        [SerializeField] private GameObject _movingEggPrefab;
        [SerializeField] private int _movingEggSpawnMinEggCollected = 4;

        [SerializeField] private GameObject _jumpingEggPrefab;
        [SerializeField] private int _jumpingEggSpawnMinEggCollected = 6;

        [SerializeField] private GameObject _goldenEggPrefab;
        [SerializeField] private int _goldenEggSpawnMinEggCollected = 10;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Found more than one Spawner Instance");
            }
            Instance = this;
            CollectedEggs = 0;
        }

        private void OnEnable()
        {
            Egg.CollectedEgg += UnlockDodge;
        }

        private void OnDisable()
        {
            Egg.CollectedEgg -= UnlockDodge;
        }

        private void UnlockDodge()
        {
            if (CollectedEggs == GameParamsLoader.EggsCollectedToUnlockDodge)
            {
                DodgeUnlocked?.Invoke();
                Egg.CollectedEgg -= UnlockDodge;
            }
        }

        public void SpawnEgg()
        {
            if (CollectedEggs < 2) return;

            GameObject prefab;

            if (CollectedEggs > _movingEggSpawnMinEggCollected && UnityEngine.Random.Range(0f,1f) < GameParamsLoader.MovingEggSpawnChance)
            {
                prefab = _movingEggPrefab;
            }
            else if (CollectedEggs > _jumpingEggSpawnMinEggCollected && UnityEngine.Random.Range(0f, 1f) < GameParamsLoader.JumpingEggSpawnChance)
            {
                prefab = _jumpingEggPrefab;
            }
            else if (CollectedEggs > _goldenEggSpawnMinEggCollected && UnityEngine.Random.Range(0f, 1f) < GameParamsLoader.GoldenEggSpawnChance)
            {
                prefab = _jumpingEggPrefab;
            }
            else
            {
                prefab = _eggPrefab;
            }

            Vector3 spawnPos = Vector3.zero.RandomNavSphere(_eggSpawnRadius, -1) + new Vector3(0, _yoffset, 0);
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _eggSpawnRadius);
        }
    }
}
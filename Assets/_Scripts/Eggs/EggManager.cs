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

        // Don't forget null check in awake
        [SerializeField] private GameObject _eggPrefab;
        [SerializeField] private GameObject _movingEggPrefab;
        [SerializeField] private GameObject _jumpingEggPrefab;
        [SerializeField] private GameObject _goldenEggPrefab;


        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Found more than one Spawner Instance");
            }
            Instance = this;
            CollectedEggs = 0;

            if (_eggPrefab == null) Debug.LogError("Missing egg prefab");
            if (_movingEggPrefab == null) Debug.LogError("Missing egg prefab");
            if (_jumpingEggPrefab == null) Debug.LogError("Missing egg prefab");
            if (_goldenEggPrefab == null) Debug.LogError("Missing egg prefab");
        }

        public void SpawnEgg()
        {
            if (CollectedEggs < 2) return;

            GameObject prefab;

            if (CollectedEggs > GameParamsLoader.MovingEggSpawnMinEggCollected && UnityEngine.Random.Range(0f,1f) < GameParamsLoader.MovingEggSpawnChance)
            {
                prefab = _movingEggPrefab;
            }
            else if (CollectedEggs > GameParamsLoader.JumpingEggSpawnMinEggCollected && UnityEngine.Random.Range(0f, 1f) < GameParamsLoader.JumpingEggSpawnChance)
            {
                prefab = _jumpingEggPrefab;
            }
            else if (CollectedEggs > GameParamsLoader.GoldenEggSpawnMinEggCollected && UnityEngine.Random.Range(0f, 1f) < GameParamsLoader.GoldenEggSpawnChance)
            {
                prefab = _goldenEggPrefab;
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
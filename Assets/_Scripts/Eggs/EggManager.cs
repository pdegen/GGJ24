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
        [SerializeField, Range(0f,1f)] private float _movingEggChance = 0.3f;
        [SerializeField] private int _movingEggSpawnMinEggCollected = 4;

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

            GameObject eggPrefab = _eggPrefab;

            if (CollectedEggs > _movingEggSpawnMinEggCollected && UnityEngine.Random.Range(0f,1f) < _movingEggChance)
            {
                eggPrefab = _movingEggPrefab;
            }

            Vector3 spawnPos = Vector3.zero.RandomNavSphere(_eggSpawnRadius, - 1) + new Vector3(0, _yoffset, 0);
            Instantiate(eggPrefab, spawnPos, Quaternion.identity);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _eggSpawnRadius);
        }
    }
}
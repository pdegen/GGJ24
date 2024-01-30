using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace GGJ24
{
    public class EggSpawner : MonoBehaviour
    {
        public static EggSpawner Instance { get; private set; }
        public static int CollectedEggs;

        [SerializeField] private float _yoffset = 0.5f;
        [SerializeField] private GameObject _eggPrefab;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Found more than one Spawner Instance");
            }
            Instance = this;
            CollectedEggs = 0;
        }

        public void SpawnEgg()
        {
            Vector3 spawnPos = GetRandomPointOnNavMesh() + new Vector3(0, _yoffset, 0);
            Instantiate(_eggPrefab, spawnPos, Quaternion.identity);
        }

        Vector3 GetRandomPointOnNavMesh()
        {
            NavMeshHit hit;
            float r = GameManager.Instance.LevelRadius;
            Vector3 randomPoint = new Vector3(Random.Range(-r, r), 0, Random.Range(-r, r));

            if (NavMesh.SamplePosition(randomPoint, out hit, 10f, NavMesh.AllAreas))
            {
                return hit.position;
            }

            return transform.position;
        }
    }
}
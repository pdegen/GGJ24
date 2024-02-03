using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace GGJ24
{
    public class EggManager : MonoBehaviour
    {
        public static EggManager Instance { get; private set; }
        public static int CollectedEggs { get; set; }

        [SerializeField] private float _eggSpawnRadius = 40f;
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
            if (CollectedEggs < 2) return;
            Vector3 spawnPos = GetRandomPointOnNavMesh() + new Vector3(0, _yoffset, 0);
            Instantiate(_eggPrefab, spawnPos, Quaternion.identity);
        }

        Vector3 GetRandomPointOnNavMesh()
        {
            float theta = Random.Range(0, 2 * Mathf.PI);
            float r = Random.Range(0, _eggSpawnRadius);
            Vector3 randomPoint = new Vector3(r*Mathf.Cos(theta), 0, r * Mathf.Sin(theta));

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                return hit.position;
            }

            return transform.position;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _eggSpawnRadius);
        }
    }
}
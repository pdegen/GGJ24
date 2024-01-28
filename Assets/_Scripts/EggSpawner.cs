using GGJ24;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EggSpawner : MonoBehaviour
{
    public static EggSpawner Instance { get; private set; }
    [SerializeField] private float _yoffset = 0.5f;

    public static int CollectedEggs = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Debug.LogWarning("Found more than one Spawner Instance");
        }
        Instance = this;
    }

    [SerializeField] private GameObject _eggPrefab;


    // Start is called before the first frame update
    public void SpawnEgg()
    {
        Vector3 spawnPos = GetRandomPointOnNavMesh() + new Vector3(0, _yoffset, 0);
        Instantiate(_eggPrefab, spawnPos, Quaternion.identity);
    }

    Vector3 GetRandomPointOnNavMesh()
    {
        NavMeshHit hit;

        // Get a random point near the current object's position
        Vector3 randomPoint = new Vector3(Random.Range(-GameManager.LevelRadius, GameManager.LevelRadius), 0, Random.Range(-GameManager.LevelRadius, GameManager.LevelRadius));

        // Sample the position on the NavMesh
        if (NavMesh.SamplePosition(randomPoint, out hit, 10f, NavMesh.AllAreas))
        {
            // If a valid point is found, return it
            return hit.position;
        }

        // If no valid point is found, return the current object's position
        return transform.position;
    }
}

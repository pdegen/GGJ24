using GGJ24;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static int EggsCollected = 0;
    private static float _levelRadius;
    public static float LevelRadius { get; private set; }  = 40f;
    [Header("Multiply speed, random shoot, bullet magnet by this factor for every egg collected")] public float RageMultiplier = 1.01f;

    private void Awake()
    {
        if (Instance == null)
        {
            Debug.LogWarning("Found more than one Spawner Instance");
        }
        Instance = this;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, LevelRadius);
    }
}

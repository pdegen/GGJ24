using DG.Tweening;
using GGJ24;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static int EggsCollected = 0;
    private static float _levelRadius;
    public static float LevelRadius { get; private set; }  = 40f;
    [Header("Multiply speed, random shoot, bullet magnet by this factor for every egg collected")] public float RageMultiplier = 1.01f;

    [Header("How many eggs to collect before changing music intensity")]
    [SerializeField] private int intensityLow = 2;
    [SerializeField] private int intensityMid = 4;
    [SerializeField] private int intensityHigh = 6;

    private void Awake()
    {
        if (Instance == null)
        {
            Debug.LogWarning("Found more than one Spawner Instance");
        }
        Instance = this;
    }

    private void OnEnable()
    {
        Egg.CollectedEgg += OnEggCollected;
    }

    private void OnDisable()
    {
        Egg.CollectedEgg -= OnEggCollected;
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        CanvasManager.Instance.ToggleGameOverScreen();
        Debug.Log("Game over");
    }

    private void OnEggCollected()
    {
        switch (Egg.CollectedEggs)
        {
            case int n when n < intensityLow:
                break;

            case int n when n >= intensityLow && n < intensityMid:
                AudioManager.Instance.SetAmbianceParameter("Intensity", 2);
                break;
            case int n when n >= intensityMid && n < intensityHigh:
                AudioManager.Instance.SetAmbianceParameter("Intensity", 3);
                break;
            case int n when n >= intensityHigh:
                AudioManager.Instance.SetAmbianceParameter("Intensity", 4);
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, LevelRadius);
    }

    public void Restart()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }
}

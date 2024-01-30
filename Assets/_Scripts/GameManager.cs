using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace GGJ24
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public float LevelRadius { get; set; } = 60f;
        public static int HighScore = 0;

        [Header("Multiply speed, random shoot, bullet magnet by this factor for every egg collected")]
        public float RageMultiplier = 1.01f;

        [Header("How many eggs to collect before changing music intensity")]
        [SerializeField] private int intensityLow = 2;
        [SerializeField] private int intensityMid = 4;
        [SerializeField] private int intensityHigh = 6;

        private StarterAssetsInputActions _inputActions;
        private bool isPaused;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Found more than one Spawner Instance");
            }
            Instance = this;
            isPaused = false;

            _inputActions = new StarterAssetsInputActions();
            _inputActions.Player.Enable();
        }
        private void OnEnable()
        {
            Egg.CollectedEgg += OnEggCollected;
            _inputActions.Player.EscaeAction.performed += TogglePause;
        }

        private void OnDisable()
        {
            Egg.CollectedEgg -= OnEggCollected;
            _inputActions.Player.EscaeAction.performed -= TogglePause;
        }

        public void EndGame()
        {
            if (EggSpawner.CollectedEggs > HighScore)
            {
                StartCoroutine(NewHighScore());
            }
            else
            {
                StartCoroutine(GameOver());
            }

        }

        private IEnumerator NewHighScore()
        {
            yield return new WaitForSeconds(4.5f);
            CanvasManager.Instance.ToggleGameOverScreen();
            AudioManager.Instance.StopAmbiance();
            Time.timeScale = 0f;
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.VictorySFX, transform.position);
            HighScore = EggSpawner.CollectedEggs;
        }

        private IEnumerator GameOver()
        {
            yield return new WaitForSeconds(4.5f);
            CanvasManager.Instance.ToggleGameOverScreen();
            AudioManager.Instance.StopAmbiance();
            Time.timeScale = 0f;
            CanvasManager.Instance.ToggleGameOverScreen();
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.GameOverSFX, transform.position);
        }

        private void OnEggCollected()
        {
            switch (EggSpawner.CollectedEggs)
            {
                case int n when n < intensityLow:
                    break;

                case int n when n >= intensityLow && n < intensityMid:
                    AudioManager.Instance.SetAmbianceParameter("Intensity", 1);
                    break;
                case int n when n >= intensityMid && n < intensityHigh:
                    AudioManager.Instance.SetAmbianceParameter("Intensity", 2);
                    break;
                case int n when n >= intensityHigh:
                    AudioManager.Instance.SetAmbianceParameter("Intensity", 3);
                    break;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, LevelRadius);
        }

        private void TogglePause(InputAction.CallbackContext context)
        {
            if (isPaused) UnpauseGame();
            else PauseGame();
        }

        public void PauseGame()
        {
            if (isPaused) return;
            Time.timeScale = 0f;
            isPaused = true;
            CanvasManager.Instance.TogglePauseScreen();
        }

        public void UnpauseGame()
        {
            if (!isPaused) return;
            Time.timeScale = 1f;
            isPaused = false;
            CanvasManager.Instance.TogglePauseScreen();
        }

        public void Restart()
        {
            AudioManager.Instance.StopAmbiance();
            Time.timeScale = 1f;
            DOTween.KillAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Threading;
using StarterAssets;

namespace GGJ24
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [field: SerializeField] public float LevelRadius { get; set; } = 60f;
        public static int HighScore = 0;

        [Header("Multiply speed, random shoot, bullet magnet by this factor for every egg collected")]
        public float RageMultiplier = 1.01f;

        [Header("How many eggs to collect before changing music intensity")]
        [SerializeField] private int intensityLow = 2;
        [SerializeField] private int intensityMid = 4;
        [SerializeField] private int intensityHigh = 6;

        public float RemainingTime { get; set; }

        private StarterAssetsInputActions _inputActions;
        private bool _isPaused;
        private bool _gameHasEnded;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Found more than one Spawner Instance");
            }
            Instance = this;
            _isPaused = false;
            _gameHasEnded = false;

            _inputActions = new StarterAssetsInputActions();
            _inputActions.Player.Enable();
        }

        private void Start()
        {
            RemainingTime = GameParamsLoader.StartTime;
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

        private void Update()
        {
            if (_gameHasEnded) { return; }
            if (RemainingTime < 0)
            {
                StartCoroutine(GameOver(0));
                return;
            }

            if (ThirdPersonController.IsDancing)
            {
                return;
            }
            RemainingTime -= Time.deltaTime;
        }

        public void EndGame()
        {
            _gameHasEnded = true;
            if (EggManager.CollectedEggs > HighScore)
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
            _gameHasEnded = true;
            yield return new WaitForSeconds(4.5f);
            CanvasManager.Instance.ToggleGameOverScreen();
            AudioManager.Instance.StopAmbiance();
            Time.timeScale = 0f;
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.VictorySFX, transform.position);
            HighScore = EggManager.CollectedEggs;
        }

        private IEnumerator GameOver(float duration = 4.5f)
        {
            _gameHasEnded = true;
            yield return new WaitForSeconds(duration);
            CanvasManager.Instance.ToggleGameOverScreen();
            AudioManager.Instance.StopAmbiance();
            Time.timeScale = 0f;
            CanvasManager.Instance.ToggleGameOverScreen();
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.GameOverSFX, transform.position);
        }

        private void OnEggCollected()
        {
            switch (EggManager.CollectedEggs)
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
            if (_isPaused) UnpauseGame();
            else PauseGame();
        }

        public void PauseGame()
        {
            if (_isPaused) return;
            Time.timeScale = 0f;
            _isPaused = true;
            CanvasManager.Instance.TogglePauseScreen();
        }

        public void UnpauseGame()
        {
            if (!_isPaused) return;
            Time.timeScale = 1f;
            _isPaused = false;
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
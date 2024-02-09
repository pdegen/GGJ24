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

        public static Action GameEnded;
        public static Action DifficultyChanged;
        public static Action TimeScaleChanged;

        [field: SerializeField] public float LevelRadius { get; set; } = 60f;

        private static int _easyHighScore { get; set; } = 0;
        private static int _normalHighScore { get; set; } = 0;
        private static int _hardHighScore { get; set; } = 0;

        [Header("Multiply speed, random shoot, bullet magnet by this factor for every egg collected")]
        public float RageMultiplier = 1.01f;

        private float _remainingTime = 0;
        public float RemainingTime
        {
            get { return _remainingTime; }
            set { _remainingTime = _remainingTime < 0 ? 0 : value; }
        }

        private StarterAssetsInputActions _inputActions;
        private bool _isPaused;
        private bool _canPause;
        private static bool _gameHasEnded;
        private bool _startTimer;


        public static Difficulty CurrentDifficulty { get; private set; }
        public enum Difficulty
        {
            EASY = 0,
            NORMAL = 1,
            HARD = 2
        }

        public static string PrintDifficulty()
        {
            switch (CurrentDifficulty)
            {
                case Difficulty.EASY: return "ANGRY";
                case Difficulty.NORMAL: return "FURIOUS";
                default: return "SATANIC";
            }

        }

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Found more than one Spawner Instance");
            }
            Instance = this;
            _isPaused = false;
            _canPause = false;
            _gameHasEnded = false;
            _startTimer = false;
            _inputActions = new StarterAssetsInputActions();
            _inputActions.Player.Enable();
        }

        private void Start()
        {
            RemainingTime = GameParamsLoader.StartTime;
            CanvasManager.Instance.ToggleDifficultySelection(true);
            Time.timeScale = 0;
            //TimeScaleChanged?.Invoke(); // can lead to script initialization order issues, temp. workaround to set scale 0 in canvasmanager
        }

        public void StartGame(int difficulty)
        {
            Time.timeScale = 1f;
            TimeScaleChanged?.Invoke();
            CurrentDifficulty = (Difficulty)difficulty;
            GameParamsLoader.AdjustDifficulty(CurrentDifficulty);
            DifficultyChanged?.Invoke();
            StartCoroutine(StartGameRoutine());
        }

        private IEnumerator StartGameRoutine()
        {
            CanvasManager.Instance.ToggleDifficultySelection(false);
            _canPause = true;
            yield return new WaitForSeconds(2.5f);
            AudioManager.Instance.StartAmbiance();
        }

        private void StartTimer()
        {
            _startTimer = true;
            Egg.CollectedEgg -= StartTimer;
        }

        private void OnEnable()
        {
            _inputActions.Player.EscaeAction.performed += TogglePause;
            Egg.CollectedEgg += StartTimer; // note: unsubscribed in StartTimer()
        }

        private void OnDisable()
        {
            _inputActions.Player.EscaeAction.performed -= TogglePause;
            Egg.CollectedEgg -= StartTimer;
        }

        private void Update()
        {
            if (_gameHasEnded) { return; }
            if (RemainingTime < 0)
            {
                RemainingTime = 0;
                EndGame();
                return;
            }
            if (_startTimer)
                RemainingTime -= Time.deltaTime;
        }

        public static float ActiveHighScore { get; private set; } = 0;
        public static float ActiveOldHighScore { get; private set; } = 0;
        private static void HandleHighscore()
        {
            if (_gameHasEnded) return; // extermely weird bug: HandleHighscore called after GameEnded?.Invoke() in EndGame()

            switch (CurrentDifficulty)
            {
                case Difficulty.EASY:
                    ActiveOldHighScore = _easyHighScore;
                    break;
                case Difficulty.NORMAL:
                    ActiveOldHighScore = _normalHighScore;
                    break;
                case Difficulty.HARD:
                    ActiveOldHighScore = _hardHighScore;
                    break;
            }

            if (EggManager.CollectedEggs <= ActiveOldHighScore)
                return;

            switch (CurrentDifficulty)
            {
                case Difficulty.EASY:
                    _easyHighScore = EggManager.CollectedEggs;
                    break;
                case Difficulty.NORMAL:
                    _normalHighScore = EggManager.CollectedEggs;
                    break;
                case Difficulty.HARD:
                    _hardHighScore = EggManager.CollectedEggs;
                    break;
            }
            ActiveHighScore = EggManager.CollectedEggs;
        }

        public void EndGame()
        {
            HandleHighscore();
            _gameHasEnded = true;
            GameEnded?.Invoke();
            if (EggManager.CollectedEggs > ActiveOldHighScore)
            {
                StartCoroutine(NewHighScore());
            }
            else
            {
                StartCoroutine(GameOver());
            }

        }

        [SerializeField] private float _gameOverDelay = 5.5f;
        private IEnumerator NewHighScore()
        {
            yield return new WaitForSeconds(_gameOverDelay);
            AudioManager.Instance.StopAmbiance();
            Time.timeScale = 0f;
            TimeScaleChanged?.Invoke();
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.VictorySFX, transform.position);
            CanvasManager.Instance.ToggleGameOverScreen();
        }

        private IEnumerator GameOver()
        {
            yield return new WaitForSeconds(_gameOverDelay);
            CanvasManager.Instance.ToggleGameOverScreen();
            AudioManager.Instance.StopAmbiance();
            Time.timeScale = 0f;
            TimeScaleChanged?.Invoke();
            CanvasManager.Instance.ToggleGameOverScreen();
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.GameOverSFX, transform.position);
        }

        public void GodMode()
        {
            RemainingTime += 1000f;
            AbilityManager.UnlockAll();
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
            if (_isPaused || _gameHasEnded || !_canPause) return;
            Time.timeScale = 0f;
            TimeScaleChanged?.Invoke();
            _isPaused = true;
            CanvasManager.Instance.TogglePauseScreen();
        }

        public void UnpauseGame()
        {
            if (!_isPaused) return;
            Time.timeScale = 1f;
            TimeScaleChanged?.Invoke();
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
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

        [field: SerializeField] public float LevelRadius { get; set; } = 60f;
        public static int HighScore = 0;
        public static int OldHighScore = 0;

        [Header("Multiply speed, random shoot, bullet magnet by this factor for every egg collected")]
        public float RageMultiplier = 1.01f;

        private float _remainingTime = 0;
        public float RemainingTime { 
            get { return _remainingTime; } 
            set { _remainingTime = _remainingTime < 0 ? 0 : value; }
        }

        private StarterAssetsInputActions _inputActions;
        private bool _isPaused;
        private bool _gameHasEnded;


        public Difficulty CurrentDifficulty;
        public enum Difficulty
        {
            EASY = 0,
            NORMAL = 1,
            HARD = 2
        }

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
            CanvasManager.Instance.ToggleDifficultySelection(true);
            Time.timeScale = 0;
        }

        public void StartGame(int difficulty)
        {
            CurrentDifficulty = (Difficulty)difficulty;
            GameParamsLoader.AdjustDifficulty(CurrentDifficulty);
            DifficultyChanged?.Invoke();
            StartCoroutine(StartGameRoutine());
        }

        private IEnumerator StartGameRoutine()
        {
            //switch (CurrentDifficulty)
            //{
            //    case Difficulty.EASY: AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ChickenMoodSFX, transform.position); break;
            //    case Difficulty.NORMAL: AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ChickenAngrySFX, transform.position); break;
            //    case Difficulty.HARD: AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ChickenWrathfulSFX, transform.position); break;
            //}
            yield return new WaitForSecondsRealtime(0f);
            CanvasManager.Instance.ToggleDifficultySelection(false);
            Time.timeScale = 1f;
            yield return new WaitForSeconds(2f);
            AudioManager.Instance.StartAmbiance();
        }

        private void OnEnable()
        {
            _inputActions.Player.EscaeAction.performed += TogglePause;
        }

        private void OnDisable()
        {
            _inputActions.Player.EscaeAction.performed -= TogglePause;
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

            //if (ThirdPersonController.IsDancing)
            //{
            //    return;
            //}
            RemainingTime -= Time.deltaTime;
        }

        public void EndGame()
        {
            _gameHasEnded = true;
            GameEnded?.Invoke();
            OldHighScore = HighScore;
            if (EggManager.CollectedEggs > OldHighScore)
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
            HighScore = EggManager.CollectedEggs;
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
            if (_isPaused || _gameHasEnded) return;
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;
using DamageNumbersPro;

namespace GGJ24
{
    public class CanvasManager : MonoBehaviour
    {
        public static CanvasManager Instance { get; private set; }

        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Transform _eggsCollected;
        [SerializeField] private TMP_Text _endGameText;
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private PlayerHealth _health;
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private GameObject _pausePanel;
        [SerializeField] private GameObject _replayButton;
        [SerializeField] private GameObject _resumeButton;
        [SerializeField] private GameObject _dodgeControlOverlay;
        [SerializeField] private TMP_Text _unlockedNotificationText;
        [SerializeField] private DamageNumber _timeBonusNumber;

        private TMP_Text _eggsText;
        private StarterAssetsInputActions _inputActions;


        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Found more than one Canvas Manager Instance");
            }
            Instance = this;

            _eggsText = _eggsCollected.GetComponent<TMP_Text>();
        }

        private void Start()
        {
            UpdateEggsText();

            _healthSlider.maxValue = _health.InitialHealth;
            _healthSlider.minValue = 0;
            _healthSlider.value = _health.InitialHealth;

            _inputActions = new StarterAssetsInputActions();
            _inputActions.Player.Enable();
            _dodgeControlOverlay.SetActive(false);
        }

        private void OnEnable()
        {
            Egg.CollectedEgg += UpdateEggsText;
            AbilityManager.AbilityUnlocked += OnAbilityUnlocked;
            PlayerHealth.TookDamage += UpdateHealth;
        }

        private void OnDisable()
        {
            Egg.CollectedEgg -= UpdateEggsText;
            AbilityManager.AbilityUnlocked -= OnAbilityUnlocked;
            PlayerHealth.TookDamage -= UpdateHealth;
        }

        private void Update()
        {
            float time = GameManager.Instance.RemainingTime;
            float minutes = Mathf.FloorToInt(time / 60);
            float seconds = Mathf.FloorToInt(time % 60);
            _timerText.text = "TIME\n" + string.Format($"{minutes:00}:{seconds:00}");
        }

        public void AddTimeBonus(float t)
        {
            DamageNumber timeNumber = _timeBonusNumber.Spawn(Vector3.zero, t);
            timeNumber.SetAnchoredPosition(_timerText.rectTransform, new Vector2(0, 0));
        }

        private void OnAbilityUnlocked(string text)
        {
            StartCoroutine(ShowNotificationRoutine(text));
        }

        private IEnumerator ShowNotificationRoutine(string text)
        {
            // TO DO: refactor this
            switch (text)
            {
                case "DODGE UNLOCKED!":
                    _dodgeControlOverlay.SetActive(true);
                    break;
            }
            _unlockedNotificationText.gameObject.SetActive(true);
            _unlockedNotificationText.text = text;
            _unlockedNotificationText.transform.DOScale(2f * transform.localScale, 0.4f);
            yield return new WaitForSeconds(2f);
            _unlockedNotificationText.GetComponent<TMP_Text>().DOFade(0, 0.3f);
        }

        public void ToggleGameOverScreen()
        {
            var eventSystem = EventSystem.current;
            eventSystem.SetSelectedGameObject(_replayButton, new BaseEventData(eventSystem));
            _inputActions.Player.Disable();
            _inputActions.UI.Enable();
            _gameOverPanel.SetActive(true);
            _eggsCollected.gameObject.SetActive(false);
            _endGameText.text = (EggManager.CollectedEggs > GameManager.HighScore) || (EggManager.CollectedEggs > 0 && GameManager.HighScore == 0) ? "NEW HIGH SCORE!" : "GAME OVER!";
            _scoreText.text = "EGGS: " + EggManager.CollectedEggs.ToString() + "\nPREVIOUS HIGH SCORE: " + GameManager.HighScore;
        }

        public void TogglePauseScreen()
        {
            if (_pausePanel.activeSelf)
            {
                _inputActions.UI.Disable();
                _inputActions.Player.Enable();
                _pausePanel.SetActive(false);
            }
            else
            {
                var eventSystem = EventSystem.current;
                eventSystem.SetSelectedGameObject(_resumeButton, new BaseEventData(eventSystem));
                _inputActions.UI.Enable();
                _inputActions.Player.Disable();
                _pausePanel.SetActive(true);
            }
        }

        public void UpdateHealth(int newValue)
        {
            _healthSlider.value = newValue;
        }

        private void UpdateEggsText()
        {
            if (EggManager.CollectedEggs > 0)
            {
                _eggsCollected.DOPunchScale(new Vector2(1.1f,1.1f), 0.6f).SetEase(Ease.InOutSine);
            }
            _eggsText.text = "EGGS: " + EggManager.CollectedEggs;
        }
    }
}
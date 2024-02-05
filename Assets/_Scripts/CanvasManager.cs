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

        [Header("Ingame UI")]
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Transform _eggsCollected;
        [SerializeField] private GameObject _controlOverlay;
        [SerializeField] private GameObject _dodgeControlOverlay;
        [SerializeField] private GameObject _dashControlOverlay;
        [SerializeField] private TMP_Text _unlockedNotificationText;
        [SerializeField] private DamageNumber _timeBonusNumber;

        [Header("Menus UI")]
        [SerializeField] private TMP_Text _endGameText;
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private PlayerHealth _health;
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private GameObject _pausePanel;
        [SerializeField] private GameObject _replayButton;
        [SerializeField] private GameObject _resumeButton;


        private TMP_Text _eggsText;
        private StarterAssetsInputActions _inputActions;
        private GameObject _currentSelectedObject;


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
            _dashControlOverlay.SetActive(false);
        }

        private void OnEnable()
        {
            Egg.CollectedEgg += UpdateEggsText;
            AbilityManager.AbilityUnlocked += OnAbilityUnlocked;
            PlayerHealth.HealthChanged += UpdateHealth;
        }

        private void OnDisable()
        {
            Egg.CollectedEgg -= UpdateEggsText;
            AbilityManager.AbilityUnlocked -= OnAbilityUnlocked;
            PlayerHealth.HealthChanged -= UpdateHealth;
        }

        private void Update()
        {
            // Update timer
            float time = GameManager.Instance.RemainingTime;
            float minutes = Mathf.FloorToInt(time / 60);
            float seconds = Mathf.FloorToInt(time % 60);
            _timerText.text = "TIME\n" + string.Format($"{minutes:00}:{seconds:00}");

            // Tweening button scale doesn't work if it's part of vertical layout group? alternative: tween color
            // Check if selection changed
            //if (Time.timeScale > 0) return;
            //var eventSystem = EventSystem.current;
            //GameObject selected = eventSystem.currentSelectedGameObject;
            //if (_currentSelectedObject != selected)
            //{
            //    OnSelectionChanged();
            //}
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
                case "DASH UNLOCKED!":
                    _dashControlOverlay.SetActive(true);
                    break;
            }
            _unlockedNotificationText.GetComponent<TMP_Text>().alpha = 1f;
            _unlockedNotificationText.gameObject.SetActive(true);
            _unlockedNotificationText.text = text;
            Vector3 priorScale = _unlockedNotificationText.transform.localScale;
            _unlockedNotificationText.transform.DOScale(1.7f * priorScale, 0.4f);
            yield return new WaitForSeconds(2f);
            _unlockedNotificationText.GetComponent<TMP_Text>().DOFade(0, 0.3f);
            yield return new WaitForSeconds(0.3f);
            _unlockedNotificationText.transform.localScale = priorScale;
        }

        public void ToggleGameOverScreen()
        {
            ToggleGameUI(false);
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
                ToggleGameUI(true);
            }
            else
            {
                var eventSystem = EventSystem.current;
                eventSystem.SetSelectedGameObject(_resumeButton, new BaseEventData(eventSystem));
                _inputActions.UI.Enable();
                _inputActions.Player.Disable();
                _pausePanel.SetActive(true);
                ToggleGameUI(false);
            }
        }

        private void ToggleGameUI(bool showUI)
        {
            _timerText.gameObject.SetActive(showUI);
            _eggsCollected.gameObject.SetActive(showUI);
            _controlOverlay.SetActive(showUI);
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

        private void OnSelectionChanged()
        {
            if (_currentSelectedObject != null)
            {
                _currentSelectedObject.transform.DOKill();
            }

            var eventSystem = EventSystem.current;
            _currentSelectedObject = eventSystem.currentSelectedGameObject;
            _currentSelectedObject.transform.DOScale(15f* _currentSelectedObject.transform.localScale.x, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }
    }
}
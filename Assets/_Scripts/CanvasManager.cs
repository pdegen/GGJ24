using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;
using DamageNumbersPro;
using FMOD.Studio;
using StarterAssets;

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
        [SerializeField] private DamageNumber _healNumber;

        [Header("Menus UI")]
        [SerializeField] private TMP_Text _endGameText;
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private PlayerHealth _health;
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private GameObject _pausePanel;
        //[SerializeField] private TMP_Text _pausePanelText;
        [SerializeField] private GameObject _replayButton;
        [SerializeField] private GameObject _resumeButton;


        private TMP_Text _eggsText;
        private StarterAssetsInputActions _inputActions;
        [SerializeField] private StarterAssetsInputs _input;
        private GameObject _currentSelectedObject;

        [Header("Button tweens")]
        [SerializeField] private Color _selectedButtonColor;
        private Vector3 _buttonBaseScale;
        private Color _buttonBaseColor;

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

            _buttonBaseColor = _resumeButton.GetComponent<Image>().color;
            _buttonBaseScale = _resumeButton.transform.localScale;
            Debug.LogWarning("Hard-coded health bar value");
            UpdateHealth(800);

            Debug.LogWarning("Setting time scale 0 in canvas manager start");
            Time.timeScale = 0f;
            OnTimeScaleChanged();
        }

        private void OnEnable()
        {
            Egg.CollectedEgg += UpdateEggsText;
            AbilityManager.AbilityUnlocked += OnAbilityUnlocked;
            PlayerHealth.HealthChanged += UpdateHealth;
            GameManager.TimeScaleChanged += OnTimeScaleChanged;
        }

        private void OnDisable()
        {
            Egg.CollectedEgg -= UpdateEggsText;
            AbilityManager.AbilityUnlocked -= OnAbilityUnlocked;
            PlayerHealth.HealthChanged -= UpdateHealth;
            GameManager.TimeScaleChanged -= OnTimeScaleChanged;
        }

        private void Update()
        {
            // Update timer
            float time = GameManager.Instance.RemainingTime;
            float minutes = Mathf.FloorToInt(time / 60);
            float seconds = Mathf.FloorToInt(time % 60);
            _timerText.text = "TIME\n" + string.Format($"{minutes:00}:{seconds:00}");
            _timerText.color = time < 10 ? Color.red : Color.white;

            // Check if selection changed
            if (Time.timeScale > 0) return;
            var eventSystem = EventSystem.current;
            GameObject selected = eventSystem.currentSelectedGameObject;
            if (_currentSelectedObject != selected)
            {
                OnSelectionChanged();
            }
        }

        [SerializeField] private GameObject _difficultySelectionMenu;
        [SerializeField] private GameObject _easyDifficultyButton;
        [SerializeField] private GameObject _normalDifficultyButton;
        [SerializeField] private GameObject _hardlDifficultyButton;
        public void ToggleDifficultySelection(bool activate)
        {
            _difficultySelectionMenu.SetActive(activate);
            if (activate)
            {
                var eventSystem = EventSystem.current;
                eventSystem.SetSelectedGameObject(_normalDifficultyButton, new BaseEventData(eventSystem));
            }
            ToggleGameUI(!activate);
        }

        public void AddTimeBonus(float t)
        {
            if (GameManager.Instance.RemainingTime <= 0) return;
            DamageNumber timeNumber = _timeBonusNumber.Spawn(Vector3.zero, t);
            timeNumber.SetAnchoredPosition(_timerText.rectTransform, new Vector2(0, 0));
        }

        private void OnAbilityUnlocked(string text)
        {
            StartCoroutine(ShowNotificationRoutine(text));
        }

        private void OnTimeScaleChanged()
        {
            if (Time.timeScale == 0)
            {
                _inputActions.Player.Disable();
                _inputActions.UI.Enable();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            } 
            else
            {
                _inputActions.Player.Enable();
                _inputActions.UI.Disable();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                _input.jump = false; // prevent jump after button press
            }
        }

        private IEnumerator ShowNotificationRoutine(string text)
        {
            // TO DO: refactor this
            switch (text)
            {
                case "DODGE UNLOCKED!":
                    _dodgeControlOverlay.SetActive(true);
                    _dodgeControlOverlay.gameObject.GetComponentInChildren<TMP_Text>(true).text = "DODGE";
                    break;
                case "DASH UNLOCKED!":
                    _dashControlOverlay.SetActive(true);
                    break;
                case "REFLECT UNLOCKED!":
                    _dodgeControlOverlay.gameObject.GetComponentInChildren<TMP_Text>(true).text = "REFLECT";
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

            _gameOverPanel.SetActive(true);
            _eggsCollected.gameObject.SetActive(false);
            _endGameText.text = (EggManager.CollectedEggs > GameManager.ActiveOldHighScore) || (EggManager.CollectedEggs > 0 && GameManager.ActiveOldHighScore == 0) ? "NEW HIGH SCORE!" : "GAME OVER!";
            _scoreText.text = "EGGS: " + EggManager.CollectedEggs.ToString() + "\nPREVIOUS HIGH SCORE: " + GameManager.ActiveOldHighScore + $"\nDifficulty: {GameManager.PrintDifficulty()}";
        }

        public void TogglePauseScreen()
        {
            if (_pausePanel.activeSelf)
            {
                _pausePanel.SetActive(false);
                ToggleGameUI(true);
            }
            else
            {
                var eventSystem = EventSystem.current;
                eventSystem.SetSelectedGameObject(_resumeButton, new BaseEventData(eventSystem));
                //_pausePanelText.text = $"EGGS: {EggManager.CollectedEggs}\nHIGH SCORE: {GameManager.HighScore}";
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
            if (newValue > _healthSlider.value)
            {
                //DamageNumber healNumber = _healNumber.Spawn(Vector3.zero, newValue - _healthSlider.value);
                //healNumber.SetAnchoredPosition(_healthSlider.gameObject.GetComponent<RectTransform>(), new Vector2(0, 0));
                _healthSlider.transform.DOPunchScale(new Vector2(1.05f, 1.05f), 0.6f).SetEase(Ease.InOutSine);
            }

            DOTween.To(() => _healthSlider.value, x => _healthSlider.value = x, newValue, 1);
        }

        private void UpdateEggsText()
        {
            if (EggManager.CollectedEggs > 0)
            {
                _eggsCollected.DOPunchScale(new Vector2(1.1f,1.1f), 0.6f).SetEase(Ease.InOutSine);
            }
            _eggsText.text = "EGGS: " + EggManager.CollectedEggs;
        }

        private EventInstance _stoppableOneShotInstance;
        private void OnSelectionChanged()
        {
            if (_currentSelectedObject != null)
            {
                _currentSelectedObject.transform.DOKill();
                _currentSelectedObject.transform.localScale = _buttonBaseScale;

                if (_currentSelectedObject.TryGetComponent<Image>(out Image img))
                {
                    img.DOKill();
                    img.color = _buttonBaseColor;
                }
            }

            var eventSystem = EventSystem.current;
            _currentSelectedObject = eventSystem.currentSelectedGameObject;

            if (_currentSelectedObject != null)
            {
                _currentSelectedObject.transform.DOScale(1.1f * _currentSelectedObject.transform.localScale.x, 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
                _currentSelectedObject.GetComponent<Image>().DOColor(_selectedButtonColor, 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
            }

            if (_stoppableOneShotInstance.isValid())
            {
                _stoppableOneShotInstance.stop(STOP_MODE.IMMEDIATE);
            }
            if (_currentSelectedObject == _easyDifficultyButton) _stoppableOneShotInstance = AudioManager.Instance.PlayStoppablOneShot(FMODEvents.Instance.ChickenNormal, transform.position);
            else if (_currentSelectedObject == _normalDifficultyButton) _stoppableOneShotInstance = AudioManager.Instance.PlayStoppablOneShot(FMODEvents.Instance.ChickenAngrySFX, transform.position);
            else if (_currentSelectedObject == _hardlDifficultyButton) _stoppableOneShotInstance = AudioManager.Instance.PlayStoppablOneShot(FMODEvents.Instance.ChickenWrathfulSFX, transform.position);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace GGJ24
{
    public class CanvasManager : MonoBehaviour
    {
        public static CanvasManager Instance { get; private set; }

        [SerializeField] private Slider _healthSlider;
        [SerializeField] private TMP_Text _eggsText;
        [SerializeField] private PlayerHealth _health;
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private GameObject _pausePanel;
        [SerializeField] private GameObject _replayButton;
        private StarterAssetsInputActions _inputActions;


        private void Awake()
        {
            if (Instance == null)
            {
                Debug.LogWarning("Found more than one Spawner Instance");
            }
            Instance = this;

        }

        private void Start()
        {
            UpdateEggsText();

            _healthSlider.maxValue = _health.InitialHealth;
            _healthSlider.minValue = 0;
            _healthSlider.value = _health.InitialHealth;

            _inputActions = new StarterAssetsInputActions();
            _inputActions.Player.Enable();
        }

        private void OnEnable()
        {
            Egg.CollectedEgg += UpdateEggsText;
            PlayerHealth.TookDamage += UpdateHealth;
        }

        private void OnDisable()
        {
            Egg.CollectedEgg -= UpdateEggsText;
            PlayerHealth.TookDamage -= UpdateHealth;
        }

        public void ToggleGameOverScreen()
        {
            _inputActions.Player.Disable();
            _inputActions.UI.Enable();
            var eventSystem = EventSystem.current;
            eventSystem.SetSelectedGameObject(_replayButton, new BaseEventData(eventSystem));
            _gameOverPanel.SetActive(true);
        }

        public void TogglePauseScreen()
        {
            if (_pausePanel.activeSelf)
            {
                _inputActions.UI.Disable();
                _inputActions.Player.Enable();
                _pausePanel.SetActive(false);
            } else
            {
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
            _eggsText.text = "Eggs: " + EggSpawner.CollectedEggs;
        }
    }
}
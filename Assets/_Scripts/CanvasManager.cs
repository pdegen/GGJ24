using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
            _gameOverPanel.SetActive(true);
        }

        public void TogglePauseScreen()
        {
            _pausePanel.SetActive(!_pausePanel.activeSelf);
        }

        public void UpdateHealth(int newValue)
        {
            _healthSlider.value = newValue;
        }

        private void UpdateEggsText()
        {
            _eggsText.text = "Eggs: " + Egg.CollectedEggs;
        }
    }
}
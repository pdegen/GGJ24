using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GGJ24
{
    public class CanvasManager : MonoBehaviour
    {
        public static CanvasManager Instance { get; private set; }

        [SerializeField] private TMP_Text _eggsText;
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
        }

        private void OnEnable()
        {
            Egg.CollectedEgg += UpdateEggsText;
        }

        private void OnDisable()
        {
            Egg.CollectedEgg -= UpdateEggsText;
        }

        private void UpdateEggsText()
        {
            _eggsText.text = "Eggs: " + Egg.CollectedEggs + "/" + Egg.TotalEggs;
        }
    }
}
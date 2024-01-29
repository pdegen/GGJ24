using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ24
{
    public class ChickenManager : MonoBehaviour
    {
        public static ChickenManager Instance { get; private set; }

        [SerializeField] private Material _chickenMaterial;
        [SerializeField] float _maxEmissionIntensity = 30f;
        [SerializeField] int _numEggsUntilMaxRageEyes = 10;

        [SerializeField] private float _wavePeriod = 2f;
        [SerializeField] private float _numChickensPerRageLevel = 2;
        [SerializeField] private bool _enableDummyWave = false;

        private static int _totalChickens;
        private static int _chickensAwake = 0;
        private float _waveTimer;



        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Found more than one Spawner Instance");
            }
            Instance = this;
        }

        private void OnEnable()
        {
            Egg.CollectedEgg += OnEggCollected;
        }

        private void OnDisable()
        {
            Egg.CollectedEgg -= OnEggCollected;
        }

        private readonly List<Chicken> _chickenList = new List<Chicken>();

        private void Start()
        {
            GetChickens();
            _waveTimer = _wavePeriod;
            if (_chickenMaterial == null)
            {
                Debug.LogWarning("Chicken material not found");
            }
            _chickenMaterial.SetColor("_EmissionColor", Color.black);
        }

        public void GetChickens()
        {
            _chickenList.Clear();
            GameObject[] chicken = GameObject.FindGameObjectsWithTag("Chicken");

            foreach (GameObject enemyObject in chicken)
            {
                Chicken chickenComponent = enemyObject.GetComponent<Chicken>();
                if (chickenComponent != null)
                {
                    _chickenList.Add(chickenComponent);
                }
            }
            _chickenList.Shuffle();
            Debug.Log("Collected " + _chickenList.Count + " chickens.");
            _totalChickens = _chickenList.Count;
        }

        public void Update()
        {
            if (_enableDummyWave) {
                _waveTimer += Time.deltaTime;
                if (_waveTimer > _wavePeriod)
                {
                    WakeUpChickens();
                    _waveTimer = 0;
                }
            }

        }

        public void WakeUpChickens()
        {
            if (_chickensAwake >= _totalChickens) return;

            int c = _chickensAwake;
            for (int i = _chickensAwake; i < Mathf.Min(_totalChickens, c + _numChickensPerRageLevel); i++)
            {
                _chickenList[i].WakeUpWrapper();
                _chickensAwake++;
                //Debug.Log("wake up chicken " + i);
            }
        }

        private void OnEggCollected()
        {
            WakeUpChickens();
            UpdateEmission();
            //UdpateBloom();
        }
        //private void UdpateBloom()
        //{
        //    float parameterValue = Mathf.Min(1f, (float)EggSpawner.CollectedEggs / (float)_numEggsUntilMaxBloom);
        //    float newBloomIntensity = Mathf.Lerp(0f, _maxBloomIntensity, parameterValue);
        //    PostProcessController.Instance.SetBloomIntensity(newBloomIntensity);
        //    Debug.Log("set bloom"  + newBloomIntensity);
        //}

        private void UpdateEmission()
        {
            float parameterValue = Mathf.Min(1f, (float)EggSpawner.CollectedEggs / (float)_numEggsUntilMaxRageEyes);

            if (_chickenMaterial != null)
            {
                float newEmissionIntensity = Mathf.Lerp(0f, _maxEmissionIntensity, parameterValue);
                Color newEmissionColor = Color.red * newEmissionIntensity;
                _chickenMaterial.SetColor("_EmissionColor", newEmissionColor);
            }
            else
            {
                Debug.LogWarning("Material not found or not initialized.");
            }
        }
    }
}
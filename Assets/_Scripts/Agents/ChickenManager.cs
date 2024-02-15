using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ24
{
    public class ChickenManager : MonoBehaviour//, IDifficultydDependable
    {
        public static ChickenManager Instance { get; private set; }

        [SerializeField] private Material _chickenEmissionMaterial;
        [SerializeField] private float _emissionIntensity = 30f;
        //private float _maxEmissionIntensityBase;
        //[SerializeField] float _startEmissionIntensity = 2f;
        //[SerializeField] int _numEggsUntilMaxRageEyes = 10;

        [SerializeField] private int _numChickensPerEggCollected = 2;

        private static int _totalChickens;
        private float _nearbyChickenSearchRadius = 4f;
        private int _chickensAwake;


        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Found more than one Spawner Instance");
            }
            Instance = this;
            _chickensAwake = 0;
        }

        private void OnEnable()
        {
            Egg.CollectedEggAtPosition += OnEggCollected;
        }

        private void OnDisable()
        {
            Egg.CollectedEggAtPosition -= OnEggCollected;
        }

        private readonly List<Chicken> _chickenList = new List<Chicken>();

        private void Start()
        {
            GetChickens();
            if (_chickenEmissionMaterial == null)
            {
                Debug.LogWarning("Chicken emission material not found");
            }
            _chickenEmissionMaterial.SetColor("_EmissionColor", _emissionIntensity * Color.red);
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

        public void WakeUpChickens(Vector3 position)
        {
            if (_chickensAwake >= _totalChickens) return;

            int nearbyChickensWokenUp = WakeUpNearbyChickens(position);

            if (nearbyChickensWokenUp < _numChickensPerEggCollected)
            {
                WakeUpRandomChickens(_numChickensPerEggCollected - nearbyChickensWokenUp);
            }
        }

        // Returns number of nearby chickens woken up
        public int WakeUpNearbyChickens(Vector3 position)
        {
            int wokenUp = 0;
            Collider[] colliders = Physics.OverlapSphere(position, _nearbyChickenSearchRadius);
            foreach (Collider hit in colliders)
            {
                if (hit.TryGetComponent(out Chicken chicken))
                {
                    if (chicken.IsSleeping) { chicken.WakeUpWrapper(); }
                    wokenUp++;
                    _chickensAwake++;
                    if (wokenUp >= _numChickensPerEggCollected) break;
                }
            }
            return wokenUp;
        }

        public void WakeUpRandomChickens(int n)
        {
            int wokenUp = 0;
            for (int i = 0; i < _totalChickens; i++)
            {
                if (_chickenList[i].IsSleeping)
                {
                    wokenUp++;
                    _chickensAwake++;
                    _chickenList[i].WakeUpWrapper();
                }
                if (wokenUp == n) return;
            }
        }

        private void OnEggCollected(Vector3 position)
        {
            WakeUpChickens(position);
            //UpdateEmission();
            //UdpateBloom();
        }
        //private void UdpateBloom()
        //{
        //    float parameterValue = Mathf.Min(1f, (float)EggSpawner.CollectedEggs / (float)_numEggsUntilMaxBloom);
        //    float newBloomIntensity = Mathf.Lerp(0f, _maxBloomIntensity, parameterValue);
        //    PostProcessController.Instance.SetBloomIntensity(newBloomIntensity);
        //    Debug.Log("set bloom"  + newBloomIntensity);
        //}

        //private void UpdateEmission()
        //{
        //    float parameterValue = Mathf.Min(1f, (float)EggManager.CollectedEggs / (float)_numEggsUntilMaxRageEyes);

        //    if (_chickenEmissionMaterial != null)
        //    {
        //        float newEmissionIntensity = Mathf.Lerp(_startEmissionIntensity, _maxEmissionIntensity, parameterValue);
        //        Color newEmissionColor = Color.red * newEmissionIntensity;
        //        _chickenEmissionMaterial.SetColor("_EmissionColor", newEmissionColor);
        //    }
        //    else
        //    {
        //        Debug.LogWarning("Material not found or not initialized.");
        //    }
        //}

        //public void RefreshDifficultyParams()
        //{
        //    _maxEmissionIntensity = _maxEmissionIntensityBase * GameParamsLoader.CurrentMultiplier;
        //}
    }
}
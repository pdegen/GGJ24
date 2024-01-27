using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ24
{
    public class ChickenManager : MonoBehaviour
    {

        //[SerializeField] private GameObject _chickenprefab;
        public static ChickenManager Instance { get; private set; }
        private static int _totalChickens;
        private static int _chickensAwake = 0;

        private float _waveTimer;
        [SerializeField] private float _wavePeriod = 2f;
        [SerializeField] private float _numChickensPerRageLevel = 2;

        [SerializeField] private bool _enableDummyWave = false;

        private void Awake()
        {
            if (Instance == null)
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
        }

        public void GetChickens()
        {
            _chickenList.Clear();
            // Find all game objects with the specified tag
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
                _chickenList[i].WakeUp();
                _chickensAwake++;
                //Debug.Log("wake up chicken " + i);
            }
        }

        private void OnEggCollected()
        {
            WakeUpChickens();
        }



        //public void SpawnChickens()
        //{
        //    if (_chickenCount > _maxChickens)
        //        return;

        //    for (int i = 0; i < _spawnBaseCount; ++i)
        //    {
        //        float x = Random.Range(0, GameManager.LevelRadius);
        //        float z = Random.Range(0, GameManager.LevelRadius);
        //        Vector3 spawnPos = new Vector3(x,0,z);
        //        Instantiate(_chickenprefab, spawnPos, Quaternion.identity);
        //        _chickenCount++;
        //    }
        //}
    }
}
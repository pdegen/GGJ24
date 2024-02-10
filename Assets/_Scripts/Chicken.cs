using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;

namespace GGJ24
{
    public class Chicken : AgentMovement, IDifficultydDependable
    {
        public bool IsSleeping { get; private set; } = true;
        [SerializeField] private float _wakeUpDuration = 1.2f;
        [SerializeField] private MeshRenderer _chickenMeshRenderer;
        [SerializeField] private Material _emissionMaterial;

        private EventInstance _ambientEventInstance;
        private bool _rageSoundsActive;
        protected override void Awake()
        {
            base.Awake();
            _rageSoundsActive = false;
        }

        protected override void Start()
        {
            base.Start();
            _shooting.CanShoot = false;
            StartCoroutine(InitAudio());
            _targeting.TargetingEnabled = false;
        }

        private IEnumerator InitAudio()
        {
            yield return new WaitForSeconds(Random.Range(0f,10f));
            _ambientEventInstance = AudioManager.Instance.CreateAttachedEventInstance(FMODEvents.Instance.ChickenMoodSFX, transform);
            _ambientEventInstance.setParameterByName("Chicken Mood", 0);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Egg.CollectedEgg += OnEggCollected;
            GameManager.DifficultyChanged += RefreshDifficultyParams;
            GameManager.GameEnded += OnGameEnded;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Egg.CollectedEgg -= OnEggCollected;
            GameManager.DifficultyChanged -= RefreshDifficultyParams;
            GameManager.GameEnded -= OnGameEnded;
        }

        protected override void Update()
        {
            if (IsSleeping) { return; }
            base.Update();
            //_ambientEventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        }

        public void WakeUpWrapper()
        {
            StartCoroutine(WakeUp());
        }

        private IEnumerator WakeUp()
        {
            _chickenMeshRenderer.material = _emissionMaterial;
            transform.parent = null;
            StartCoroutine(ActivateAngryChickenSounds(5f));
            transform.DOLocalJump(transform.position + transform.TransformDirection(new Vector3(0, -0.42f, 2)), 2.5f, 1, _wakeUpDuration);
            yield return new WaitForSeconds(_wakeUpDuration);
            _agent.enabled = true;
            IsSleeping = false;
            _targeting.TargetingEnabled = true;
            _shooting.CanShoot = true;
            SetNewDestination();
        }

        private void OnEggCollected()
        {
            _agent.speed *= GameManager.Instance.RageMultiplier;
        }

        private void OnGameEnded()
        {
            _ambientEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }

        protected override void CommenceHostilities()
        {
            base.CommenceHostilities();
            if (_agent.enabled)
            {
                StartCoroutine(ActivateAngryChickenSounds(5f));
            }
        }

        private IEnumerator ActivateAngryChickenSounds(float duration)
        {
            if (_rageSoundsActive || GameManager.CurrentDifficulty == GameManager.Difficulty.EASY) yield break;
            _rageSoundsActive = true;
            _ambientEventInstance.setParameterByName("Chicken Mood", 2);
            yield return new WaitForSeconds(duration);
            _ambientEventInstance.setParameterByName("Chicken Mood", 0);
            _rageSoundsActive = false;
        }

        public void RefreshDifficultyParams()
        {
            _agentDisableDuration = GameParamsLoader.ChickenKnockoutDuration;
            _agent.speed = _speed * GameParamsLoader.CurrentMultiplier;
        }
    }
}
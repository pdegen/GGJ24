using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using FMOD.Studio;

namespace GGJ24
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Chicken : MonoBehaviour, IDifficultydDependable
    {
        public bool IsSleeping { get; private set; } = true;
        public bool IsMovedByRigidBody { get; private set; } = false;

        private static Vector3 _spawnPoint = Vector3.forward;

        [SerializeField] protected float _wanderDuration;
        [SerializeField] protected Transform _destinationGizmo;
        [SerializeField, Min(0.1f)] protected float _pathUpdateSpeed = 0.5f;
        [SerializeField] private float _rotationSpeed = 10f;
        [SerializeField] private float _recoilForce = 2f;
        [SerializeField] private float _recoilTorque = 2f;
        [SerializeField] private float _wakeUpDuration = 1.2f;

        [SerializeField] private MeshRenderer _chickenMeshRenderer;
        [SerializeField] private Material _emissionMaterial;

        protected Vector3 _destinationPos;
        protected NavMeshAgent _agent;
        private float _speed;
        protected delegate IEnumerator GetNewDestination();
        protected GetNewDestination _autoNewDestination;

        [SerializeField] private Bazooka _bazooka;
        [SerializeField] private Shooting _shooting;

        [SerializeField] private float _targetDistanceHostile = 15f;
        private float _targetDistanceReached;
        private readonly float _targetDistanceNeutral = 0.3f;
        private float _agentDisableDuration;
        [SerializeField] private ChickenState _state;
        private float _oobRadiusSquared;
        private Rigidbody _body;
        private Coroutine NavmeshDisabledRoutine;
        private EventInstance _ambientEventInstance;
        private bool _rageSoundsActive;

        private enum ChickenState
        {
            Neutral = 0,
            Hostile = 1
        }

        protected virtual void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _body = GetComponent<Rigidbody>();
            _agent.enabled = false;
            _targetDistanceReached = _targetDistanceNeutral;
            _rageSoundsActive = false;
        }

        protected virtual void Start()
        {
            _bazooka.gameObject.SetActive(false);
            _state = ChickenState.Neutral;
            _shooting.CanShoot = false;
            _agent.speed *= Random.Range(1f, 5f);
            _agent.acceleration *= Random.Range(0.8f, 2f);
            _agent.angularSpeed *= Random.Range(0.8f, 2f);
            _oobRadiusSquared = GameManager.Instance.LevelRadius * GameManager.Instance.LevelRadius;
            _agentDisableDuration = GameParamsLoader.ChickenKnockoutDuration;
            _speed = _agent.speed;

            if (_spawnPoint == Vector3.forward)
            {
                GameObject[] _spawns = GameObject.FindGameObjectsWithTag("SpawnPoint");
                if (_spawns.Length > 0)
                {
                    _spawnPoint = _spawns[0].transform.position;
                }
                else
                {
                    Debug.LogWarning("No chicken spawn points found");
                    _spawnPoint = Vector3.zero;
                }
            }

            StartCoroutine(InitAudio());
        }

        private IEnumerator InitAudio()
        {
            _ambientEventInstance = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.ChickenMoodSFX);
            _ambientEventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform));
            yield return new WaitForSeconds(Random.Range(0f,10f));
            _ambientEventInstance.setParameterByName("Chicken Mood", 0);
            _ambientEventInstance.start();
        }

        private void OnEnable()
        {
            _bazooka.TargetStateChanged += OnTargetStateChanged;
            Egg.CollectedEgg += OnEggCollected;
            GameManager.DifficultyChanged += RefreshDifficultyParams;
            GameManager.GameEnded += OnGameEnded;
        }

        private void OnDisable()
        {
            _bazooka.TargetStateChanged -= OnTargetStateChanged;
            Egg.CollectedEgg -= OnEggCollected;
            GameManager.DifficultyChanged -= RefreshDifficultyParams;
            GameManager.GameEnded -= OnGameEnded;
        }

        // Update is called once per frame
        protected virtual void Update()
        {

            if (IsSleeping || IsMovedByRigidBody) return;
            if (IsOOB())
            {
                //Debug.Log("OOB, resetting");
                ResetChicken();
            }
            if (!_agent.enabled  || !_agent.isOnNavMesh) return;

            HandleDistanceCheck();
        }

        private void HandleDistanceCheck()
        {
            try
            {
                if (_agent.remainingDistance < _targetDistanceReached)
                {
                    switch (_state)
                    {
                        case ChickenState.Neutral:
                            TargetReached();
                            break;
                        case ChickenState.Hostile:
                            _agent.isStopped = true;
                            RotateTowardsTarget();
                            break;
                    }
                }
                else
                {
                    _agent.isStopped = false;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Unable to handle distance check, resetting:" + ex);
                ResetChicken();
            }
        }

        bool IsOOB()
        {
            return transform.position.sqrMagnitude > _oobRadiusSquared;
        }

        void RotateTowardsTarget()
        {
            Vector3 direction = (_bazooka.Target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _rotationSpeed);
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
            _bazooka.gameObject.SetActive(true);
            _shooting.CanShoot = true;
            SetNewDestination();
        }

        private void OnEggCollected()
        {
            _agent.speed *= GameManager.Instance.RageMultiplier;
        }

        private void OnGameEnded()
        {
            _ambientEventInstance.stop(STOP_MODE.IMMEDIATE);
        }

        private void OnTargetStateChanged()
        {
            if (IsMovedByRigidBody)
            {
                if (_state == ChickenState.Hostile)
                    EndHostilities();
                return;
            }

            switch(_bazooka.State)
            {
                case Bazooka.BazookaState.Neutral:
                    EndHostilities();
                    break;
                case Bazooka.BazookaState.Hostile:
                    CommenceHostilities();
                    break;
            }
        }

        private void EndHostilities()
        {
            _shooting.IsHostile = false;
            _state = ChickenState.Neutral;
            _targetDistanceReached = _targetDistanceNeutral;
        }
        private void CommenceHostilities()
        {
            _targetDistanceReached = _targetDistanceHostile;
            _state = ChickenState.Hostile;
            if (_agent.enabled)
            {
                _agent.SetDestination(_bazooka.Target.position);
                StartCoroutine(ActivateAngryChickenSounds(5f));
            }
            else
            {
                Debug.LogWarning("Hostilities failed, resetting");
                ResetChicken();
                _agent.enabled = true;
            }
        }

        protected virtual void SetNewDestination()
        {
            if (!_agent.enabled || IsMovedByRigidBody || !_agent.isOnNavMesh)
            {
                return;
            }

            try
            {
                _destinationPos = Vector3.zero.RandomNavSphere(GameManager.Instance.LevelRadius, -1);
                _agent.SetDestination(_destinationPos);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error setting destination, resetting: " + ex.Message);
                ResetChicken();
            }
        }

        private void ResetChicken()
        {
            _agent.enabled = false; // could use NavMeshAgent.Warp(Vector3 newPosition) but not certain that _agent is enabled
            transform.position = _spawnPoint;
            EndHostilities();
            IsMovedByRigidBody = false;
            _agent.enabled = true;
            TargetReached();
        }

        protected virtual void TargetReached()
        {
            SetNewDestination();
        }

        public void Recoil()
        {
            if (NavmeshDisabledRoutine != null) return;

            NavmeshDisabledRoutine = StartCoroutine(TemporarilyDisableNavMeshAgent(_agentDisableDuration));
            _body.AddForce(_recoilForce * transform.TransformDirection(new Vector3(0, 0.4f, -1)), ForceMode.Impulse);
            float rnd = Random.Range(-1f, 1f);
            _body.AddTorque(_recoilTorque * new Vector3(rnd, rnd, rnd), ForceMode.Impulse);
        }

        public void TemporarilyDisableNavMeshAgentWrapper()
        {
            NavmeshDisabledRoutine ??= StartCoroutine(TemporarilyDisableNavMeshAgent(_agentDisableDuration));
        }
        public IEnumerator TemporarilyDisableNavMeshAgent(float duration = 1f)
        {
            //Debug.Log("disabling navmesh for " + duration + " s");
            IsMovedByRigidBody = true;
            _body.isKinematic = false;
            _agent.enabled = false;
            _shooting.CanShoot = false;
            yield return new WaitForSeconds(duration);
            _shooting.CanShoot = true;
            _body.velocity = Vector3.zero;
            _body.isKinematic = true;
            ResetRotation();
            IsMovedByRigidBody = false;
            NavmeshDisabledRoutine = null;

            if (NavMesh.SamplePosition(transform.position, out NavMeshHit closestHit, 500, 1))
            {
                transform.position = closestHit.position;
                _agent.enabled = true;
                SetNewDestination();
            }
            else
            {
                ResetChicken();
            }
        }

        private void ResetRotation()
        {
            transform.DOLocalJump(transform.position, 1, 1, 0.8f);
            transform.DORotateQuaternion(Quaternion.identity, 0.8f);
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
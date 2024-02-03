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
    public class Chicken : MonoBehaviour
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
        [SerializeField] private float _agentDisableDuration = 3f;
        [SerializeField] private float _wakeUpDuration = 1.2f;

        [SerializeField] private MeshRenderer _chickenMeshRenderer;
        [SerializeField] private Material _emissionMaterial;

        protected Vector3 _destinationPos;
        protected NavMeshAgent _agent;
        protected delegate IEnumerator GetNewDestination();
        protected GetNewDestination _autoNewDestination;

        [SerializeField] private Bazooka _bazooka;
        [SerializeField] private Shooting _shooting;

        [SerializeField] private float _targetDistanceHostile = 15f;
        private float _targetDistanceReached;
        private readonly float _targetDistanceNeutral = 0.3f;

        [SerializeField] private ChickenState _state;
        private float _oobRadiusSquared;
        private Rigidbody _body;
        private Coroutine NavmeshDisabledRoutine;
        private EventInstance ambientEventInstance;

        private enum ChickenState
        {
            Neutral = 0,
            Hostile = 1
        }

        protected virtual void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.enabled = false;
            _targetDistanceReached = _targetDistanceNeutral;
        }

        protected virtual void Start()
        {
            _bazooka.gameObject.SetActive(false);
            _state = ChickenState.Neutral;
            _body = GetComponent<Rigidbody>();
            _shooting.CanShoot = false;
            _agent.speed *= Random.Range(1f, 5f);
            _agent.acceleration *= Random.Range(0.8f, 2f);
            _agent.angularSpeed *= Random.Range(0.8f, 2f);
            _oobRadiusSquared = GameManager.Instance.LevelRadius * GameManager.Instance.LevelRadius;

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
            // TO DO: Angry chicken sounds
            ambientEventInstance = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.ChickenMoodSFX);
            ambientEventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform));
            yield return new WaitForSeconds(Random.Range(0f,10f));
            ambientEventInstance.start();
        }

        private void OnEnable()
        {
            _bazooka.TargetStateChanged += OnTargetStateChanged;
            Egg.CollectedEgg += OnEggCollected;
        }

        private void OnDisable()
        {
            _bazooka.TargetStateChanged -= OnTargetStateChanged;
            Egg.CollectedEgg -= OnEggCollected;
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
                _destinationPos = RandomNavSphere(Vector3.zero, GameManager.Instance.LevelRadius, -1);
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
            transform.position = Vector3.zero;
            EndHostilities();
        }

        protected virtual void TargetReached()
        {
            SetNewDestination();
        }

        public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
        {
            Vector3 randDirection = Random.insideUnitSphere * dist;

            randDirection += origin;

            NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);

            return navHit.position;
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
            _agent.enabled = true;
            //Debug.Log("nav mesh enabled");
            NavmeshDisabledRoutine = null;
            IsMovedByRigidBody = false;
            SetNewDestination();
        }

        private void ResetRotation()
        {
            transform.DOLocalJump(transform.position, 1, 1, 0.8f);
            transform.DORotateQuaternion(Quaternion.identity, 0.8f);
        }
    }
}
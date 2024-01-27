using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

namespace GGJ24
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Chicken : MonoBehaviour
    {
        [SerializeField] protected float _wanderDuration;
        [SerializeField] protected Transform _destinationGizmo;
        [SerializeField, Min(0)] protected float _maxDistance = 60f;
        [SerializeField, Min(0.1f)] protected float _pathUpdateSpeed = 0.5f;
        [SerializeField] private float _rotationSpeed = 10f;
        [SerializeField] private float _agentDisableDuration = 3f;

        protected Vector3 _destinationPos;
        protected NavMeshAgent _agent;
        protected Coroutine _pathRoutine;
        protected delegate IEnumerator GetNewDestination();
        protected GetNewDestination _autoNewDestination;
        [SerializeField] private Bazooka _bazooka;
        [SerializeField] private Shooting _shooting;

        [SerializeField] private Transform _spawnCenter;

        [SerializeField] private float _targetDistanceHostile = 15f;
        private float _targetDistanceReached;
        private readonly float _targetDistanceNeutral = 0.3f;

        public bool IsSleeping = true;
        public bool IsMovedByRigidBody = false;
        [SerializeField] private ChickenState _state;

        private Rigidbody _body;

        public Coroutine NavmeshDisabledRoutine;

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

        // Start is called before the first frame update
        protected virtual void Start()
        {
            _autoNewDestination = AutoNewDestination;
            _destinationGizmo.gameObject.SetActive(false);
            if (_destinationGizmo != null) _destinationGizmo.transform.parent = null;
            _bazooka.gameObject.SetActive(false);
            _state = ChickenState.Neutral;
            _body = GetComponent<Rigidbody>();
            _shooting.CanShoot = false;
            _agent.speed *= Random.Range(1f, 10f);
            _agent.acceleration *= Random.Range(1f, 2f);
            _agent.angularSpeed *= Random.Range(1f, 2f);
        }

        private void OnEnable()
        {
            _bazooka.TargetStateChanged += OnTargetStateChanged;
        }

        private void OnDisable()
        {
            _bazooka.TargetStateChanged -= OnTargetStateChanged;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            // When falling below map?
            if (transform.position.y < -10) transform.position = new Vector3(transform.position.x, 1, transform.position.z);

            if (IsSleeping || IsMovedByRigidBody) return;
            if (!_agent.enabled) return;

            HandleDistanceCheck();
        }

        private void HandleDistanceCheck()
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

        bool IsAgentOnNavMesh()
        {
            NavMeshHit hit;
            return NavMesh.SamplePosition(transform.position, out hit, 0.1f, NavMesh.AllAreas);
        }

        void RotateTowardsTarget()
        {
            Vector3 direction = (_bazooka.Target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _rotationSpeed);
        }

        private void OnTargetStateChanged()
        {
            if (IsMovedByRigidBody)
            {
                if (_state == ChickenState.Hostile)
                    ReturnToNeutral();
                return;
            }

            switch(_bazooka.State)
            {
                case Bazooka.BazookaState.Neutral:
                    _shooting.IsHostile = false;
                    _state = ChickenState.Neutral;
                    _targetDistanceReached = _targetDistanceNeutral;
                    if (_pathRoutine != null)
                        StopCoroutine(_pathRoutine);
                    _pathRoutine = StartCoroutine(_autoNewDestination());
                    break;
                case Bazooka.BazookaState.Hostile:
                    ReturnToNeutral();
                    break;
            }
            //Debug.Log("state changed: " + _state);
        }

        private void ReturnToNeutral()
        {
            _targetDistanceReached = _targetDistanceHostile;
            _state = ChickenState.Hostile;
            if (_pathRoutine != null)
                StopCoroutine(_pathRoutine);
            if (_agent.enabled)
            {
                _agent.SetDestination(_bazooka.Target.position);
            }
            else
            {
                transform.position = Vector3.zero;
                _agent.enabled = true;
            }
        }

        public void WakeUp()
        {
            transform.parent = null;
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            _agent.enabled = true   ;
            IsSleeping = false;
            _pathRoutine = StartCoroutine(_autoNewDestination());
            _bazooka.gameObject.SetActive(true);
            _destinationGizmo.gameObject.SetActive(true);
            if (_destinationGizmo != null) _destinationGizmo.transform.parent = null;
            _shooting.CanShoot = true;
        }

        protected virtual void SetNewDestination()
        {
            if (!_agent.enabled || IsMovedByRigidBody)
            {
                return;
            }

            try
            {
                _destinationPos = RandomNavSphere(Vector3.zero, _maxDistance, -1);
                _agent.SetDestination(_destinationPos);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error setting destination: " + ex.Message);
            }
            if (_destinationGizmo != null) _destinationGizmo.position = _destinationPos;
        }

        protected virtual void TargetReached()
        {
            SetNewDestination();
        }
        protected virtual IEnumerator AutoNewDestination()
        {

            if (IsSleeping || IsMovedByRigidBody) yield break;

            WaitForSeconds wait = new(_pathUpdateSpeed);
            SetNewDestination();
            float nextTime = Time.time + _wanderDuration;

            while (_autoNewDestination == AutoNewDestination)
            {
                if (Time.time > nextTime)
                {
                    SetNewDestination();
                    nextTime = Time.time + _wanderDuration;
                }
                yield return wait;
            }
        }

        public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
        {
            Vector3 randDirection = Random.insideUnitSphere * dist;

            randDirection += origin;

            NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);

            return navHit.position;
        }

        public void CoroutineWrapper()
        {
            if (NavmeshDisabledRoutine == null)
            {
                NavmeshDisabledRoutine = StartCoroutine(TemporarilyDisableNavMesh(_agentDisableDuration));
            }
        }
        public IEnumerator TemporarilyDisableNavMesh(float duration = 1f)
        {
            //Debug.Log("disabling navmesh for " + duration + " s");
            IsMovedByRigidBody = true;
            _body.isKinematic = false;
            _agent.enabled = false;
            _shooting.CanShoot = false;
            //_shooting.IsHostile = false;
            yield return new WaitForSeconds(duration);
            _shooting.CanShoot = true;
            _body.velocity = Vector3.zero;
            _body.isKinematic = true;
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            _agent.enabled = true;
            //Debug.Log("nav mesh enabled");
            NavmeshDisabledRoutine = null;
            IsMovedByRigidBody = false;
            SetNewDestination();
        }
    }
}
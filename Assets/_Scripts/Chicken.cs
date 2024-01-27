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

        protected Vector3 _destinationPos;
        protected NavMeshAgent _agent;
        protected Coroutine _pathRoutine;
        protected delegate IEnumerator GetNewDestination();
        protected GetNewDestination _autoNewDestination;
        [SerializeField] private Bazooka _bazooka;

        [SerializeField] private float _targetDistanceHostile = 15f;
        private float _targetDistanceReached;
        private readonly float _targetDistanceNeutral = 0.3f;

        private bool _isSleeping = true;
        [SerializeField] private ChickenState _state;
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
            if (_destinationGizmo != null) _destinationGizmo.transform.parent = null;
            _bazooka.gameObject.SetActive(false);
            _state = ChickenState.Neutral;
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
            if (_isSleeping) return;

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

        void RotateTowardsTarget()
        {
            Vector3 direction = (_bazooka.Target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _rotationSpeed);
        }

        private void OnTargetStateChanged()
        {
            switch(_bazooka.State)
            {
                case Bazooka.BazookaState.Neutral: 
                    _state = ChickenState.Neutral;
                    _targetDistanceReached = _targetDistanceNeutral;
                    StopCoroutine(_pathRoutine);
                    _pathRoutine = StartCoroutine(_autoNewDestination());
                    break;
                case Bazooka.BazookaState.Hostile:
                    _targetDistanceReached = _targetDistanceHostile;
                    _state = ChickenState.Hostile;
                    StopCoroutine(_pathRoutine);
                    _agent.SetDestination(_bazooka.Target.position);
                    break;
            }
            Debug.Log("state changed: " + _state);
        }

        public void WakeUp()
        {
            _agent.enabled = true   ;
            _isSleeping = false;
            _pathRoutine = StartCoroutine(_autoNewDestination());
            transform.parent = null;
            _bazooka.gameObject.SetActive(true);
        }

        protected virtual void SetNewDestination()
        {
            _destinationPos = RandomNavSphere(Vector3.zero, _maxDistance, -1);
            _agent.SetDestination(_destinationPos);
            if (_destinationGizmo != null) _destinationGizmo.position = _destinationPos;
        }

        protected virtual void TargetReached()
        {
            SetNewDestination();
        }

        protected virtual IEnumerator AutoNewDestination()
        {

            if (_isSleeping) yield break;

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
    }
}
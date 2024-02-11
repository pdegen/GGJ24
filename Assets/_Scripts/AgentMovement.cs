using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GGJ24
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Targeting))]
    public class AgentMovement : MonoBehaviour
    {

        public bool IsMovedByRigidBody { get; private set; } = false;
        [Tooltip("Reset to this point when rigidbody fails to connect to navmesh")]
        protected static Vector3 _spawnPoint;
        [SerializeField] protected float _rotationSpeed = 10f;
        [SerializeField] protected float _recoilForce = 2f;
        [SerializeField] protected float _recoilTorque = 2f;
        protected Vector3 _destinationPos;
        protected NavMeshAgent _agent;
        protected float _speed;
        protected delegate IEnumerator GetNewDestination();
        protected GetNewDestination _autoNewDestination;
        [SerializeField] private float _targetDistanceHostile = 15f;
        protected float _targetDistanceReached;
        protected readonly float _targetDistanceNeutral = 0.3f;
        protected float _agentDisableDuration;
        protected Rigidbody _body;
        protected bool _hasRigidbody;
        [SerializeField] protected AgentState _state;
        protected float _oobRadiusSquared;
        [SerializeField] protected string _spawnPointTagName;
        protected Transform _target;
        protected Targeting _targeting;
        private Coroutine NavmeshDisabledRoutine;
        protected Shooting _shooting;

        protected enum AgentState
        {
            Neutral = 0,
            Hostile = 1
        }

        protected virtual void Awake()
        {
            _hasRigidbody = TryGetComponent(out _body);
            _agent = GetComponent<NavMeshAgent>();
            _targeting = GetComponent<Targeting>();
            _agent.enabled = false;
            _targetDistanceReached = _targetDistanceNeutral;
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            _shooting = _targeting.Shooting;
            _state = AgentState.Neutral;
            _agent.speed *= Random.Range(1f, 5f);
            _agent.acceleration *= Random.Range(0.8f, 2f);
            _agent.angularSpeed *= Random.Range(0.8f, 2f);
            _oobRadiusSquared = GameManager.Instance.LevelRadius * GameManager.Instance.LevelRadius;
            _agentDisableDuration = GameParamsLoader.ChickenKnockoutDuration;
            _speed = _agent.speed;
            _target = _targeting.Target;

            if (_spawnPointTagName != null && _spawnPointTagName != "")
            {
                GameObject[] _spawns = GameObject.FindGameObjectsWithTag(_spawnPointTagName);
                if (_spawns.Length > 0)
                {
                    _spawnPoint = _spawns[0].transform.position;
                }
                else
                {
                    Debug.LogWarning("No spawn points found");
                    _spawnPoint = Vector3.zero;
                }
            }
            else
            {
                Debug.LogWarning("No spawn points found");
                _spawnPoint = Vector3.zero;
            }
        }

        protected virtual void OnEnable()
        {
            _targeting.TargetStateChanged += OnTargetStateChanged;
        }

        protected virtual void OnDisable()
        {
            _targeting.TargetStateChanged -= OnTargetStateChanged;
        }

        protected virtual void Update()
        {
            if (IsMovedByRigidBody) return;
            if (IsOOB())
            {
                //Debug.Log("OOB, resetting");
                ResetAgent();
            }
            if (!_agent.enabled || !_agent.isOnNavMesh) return;

            if (_state == AgentState.Neutral)
            {
                _agent.isStopped = false;
                if (_agent.remainingDistance < _targetDistanceReached) TargetReached();
            }
            else if (_state == AgentState.Hostile)
            {
                _agent.isStopped = true;
                RotateTowardsTarget();
            }

            //HandleDistanceCheck();
        }

        //private void HandleDistanceCheck()
        //{

        //    switch (_state)
        //    {
        //        case AgentState.Neutral:
        //            if (_agent.remainingDistance < _targetDistanceReached) TargetReached(); break;
        //        case AgentState.Hostile:
        //            _agent.isStopped = true;
        //            RotateTowardsTarget();
        //            break;
        //    }
        //    return;

        //    try
        //    {
        //        if (_agent.remainingDistance < _targetDistanceReached)
        //        {
        //            switch (_state)
        //            {
        //                case AgentState.Neutral:
        //                    TargetReached();
        //                    break;
        //                case AgentState.Hostile:
        //                    _agent.isStopped = true;
        //                    RotateTowardsTarget();
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            _agent.isStopped = false;
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Debug.LogWarning("Unable to handle distance check, resetting:" + ex);
        //        ResetAgent();
        //    }
        //}

        protected bool IsOOB()
        {
            return transform.position.sqrMagnitude > _oobRadiusSquared;
        }

        protected void RotateTowardsTarget()
        {
            Vector3 direction = (_target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _rotationSpeed);
        }

        private void OnTargetStateChanged()
        {
            if (IsMovedByRigidBody)
            {
                if (_state == AgentState.Hostile)
                    EndHostilities();
                return;
            }

            switch(_targeting.State)
            {
                case Targeting.TargetingState.Neutral:
                    EndHostilities();
                    break;
                case Targeting.TargetingState.Hostile:
                    CommenceHostilities();
                    break;
            }
        }

        private void EndHostilities()
        {
            _shooting.IsHostile = false;
            _state = AgentState.Neutral;
            _targetDistanceReached = _targetDistanceNeutral;
        }
        protected virtual void CommenceHostilities()
        {
            _targetDistanceReached = _targetDistanceHostile;
            _state = AgentState.Hostile;
            if (_agent.enabled)
            {
                _agent.SetDestination(_targeting.Target.position);
            }
            else
            {
                Debug.LogWarning("Hostilities failed, resetting");
                ResetAgent();
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
                ResetAgent();
            }
        }

        private void ResetAgent()
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
            if (NavmeshDisabledRoutine != null || !_hasRigidbody) return;

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
            if (_hasRigidbody)
            {
                IsMovedByRigidBody = true;
                _body.isKinematic = false;
            }
            _agent.enabled = false;
            _shooting.CanShoot = false;
            yield return new WaitForSeconds(duration);
            _shooting.CanShoot = true;
            if (_hasRigidbody)
            {
                _body.velocity = Vector3.zero;
                _body.isKinematic = true;
                IsMovedByRigidBody = false;
            }

            ResetRotation();
            NavmeshDisabledRoutine = null;

            if (NavMesh.SamplePosition(transform.position, out NavMeshHit closestHit, 500, 1))
            {
                transform.position = closestHit.position;
                _agent.enabled = true;
                SetNewDestination();
            }
            else
            {
                ResetAgent();
            }
        }

        private void ResetRotation()
        {
            transform.DOLocalJump(transform.position, 1, 1, 0.8f);
            transform.DORotateQuaternion(Quaternion.identity, 0.8f);
        }
    }
}
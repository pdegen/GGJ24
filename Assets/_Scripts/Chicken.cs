using System.Collections;
using System.Collections.Generic;
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
        protected Vector3 _destinationPos;
        protected NavMeshAgent _agent;
        protected Coroutine _pathRoutine;
        protected delegate IEnumerator GetNewDestination();
        protected GetNewDestination _autoNewDestination;

        private bool _isSleeping = true;

        protected virtual void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.enabled = false;
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            _autoNewDestination = AutoNewDestination;
            if (_destinationGizmo != null) _destinationGizmo.transform.parent = null;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (_isSleeping) return;

            if (_agent.remainingDistance < 0.3)
            {
                TargetReached();
            }
        }

        public void WakeUp()
        {
            _agent.enabled = true;
            _isSleeping = false;
            _pathRoutine = StartCoroutine(_autoNewDestination());
            transform.parent = null;

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
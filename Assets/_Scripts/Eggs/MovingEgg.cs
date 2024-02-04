using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GGJ24
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class MovingEgg : Egg
    {

        private NavMeshAgent _agent;
        private readonly float _targetDistanceReached = 1f;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        protected override void Start()
        {
            _agent.SetDestination(Vector3.zero.RandomNavSphere(GameManager.Instance.LevelRadius, -1));
        }

        private void Update()
        {
            if (_agent.remainingDistance < _targetDistanceReached)
            {
                _agent.SetDestination(Vector3.zero.RandomNavSphere(GameManager.Instance.LevelRadius, -1));
            }
        }
    }
}
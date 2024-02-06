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
        private float _speed;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        protected override void Start()
        {
            base.Start();
            _agent.speed = GameParamsLoader.MovingEggSpeed;
        }

        protected override void InitEggMovement()
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

        public override void RefreshDifficultyParams()
        {
            base.RefreshDifficultyParams();
            _agent.speed = GameParamsLoader.MovingEggSpeed;
        }
    }
}
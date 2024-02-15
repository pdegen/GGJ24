using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ24
{
    public class TankMovement : AgentMovement
    {
        private float _pathUpdatePeriod = 1f;
        private float _pathUpdateTimer = 0f;

        protected override void HandleDistanceCheck()
        {
            if (_state == AgentState.Neutral)
            {
                _agent.isStopped = false;
                if (_agent.remainingDistance < _targetDistanceReached) TargetReached();
            }
            else if (_state == AgentState.Hostile)
            {
                _pathUpdateTimer += Time.deltaTime;

                if (_pathUpdateTimer > _pathUpdatePeriod)
                {
                    _agent.SetDestination(_target.position);
                }

                if (_agent.remainingDistance < _targetDistanceHostile)
                {
                    _agent.isStopped = true;
                }
                else
                {
                    _agent.isStopped = false;
                }

                RotateTowardsTarget();
            }
        }

        protected override void EndHostilities()
        {
            base.EndHostilities();
            _pathUpdateTimer = 0;
        }
    }
}
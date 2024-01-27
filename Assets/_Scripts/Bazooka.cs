using System;
using UnityEngine;
using System.Collections;

namespace GGJ24
{
    public class Bazooka : MonoBehaviour
    {
        [SerializeField] private GameObject _firePoint;
        [SerializeField] private Transform _target;
        [SerializeField] private LayerMask _targetLayer;
        [SerializeField] private LayerMask _ignoreLayers;

        [Header("Cone angle theta"), Range(0f, 85f)]
        [SerializeField] private float _coneAngle = 45f;
        [SerializeField] private float _coneRangeSquared = 400f;


        [Tooltip("Wait this many seconds after target lost until return to neutral, also alert duration")]
        [SerializeField] private float _targetLostDelay = 5f;
        private float _alertTimer = 0f;

        private float _coneRange;
        private bool _targetIsInCone = false;
        private bool _targetAcquried = false;
        private Coroutine _returnToNeutralRoutine;

        private BazookaState _state;
        public BazookaState State { get => _state; private set => _state = value; }
        private Shooting _shooting;

        public event Action TargetStateChanged;

        public enum BazookaState
        {
            Neutral = 0,
            Hostile = 1,
            Alert = 2
        }

        private void Awake()
        {
            _coneRange = Mathf.Sqrt(_coneRangeSquared);
        }

        private void Start()
        {
            _shooting = _firePoint.GetComponent<Shooting>();
            _shooting.IsHostile = false;
            _target = GameObject.FindWithTag("Player").transform;
        }

        void Update()
        {
            HandleTargeting();
        }

        private void HandleTargeting()
        {
            _targetIsInCone = IsInCone();
            bool isHostile = State == BazookaState.Hostile;
            bool isAlert = State == BazookaState.Alert;

            _targetAcquried = _targetIsInCone && !IsObstructed();

            if (_targetAcquried && _returnToNeutralRoutine != null)
            {
                StopCoroutine(_returnToNeutralRoutine);
                _returnToNeutralRoutine = null;
            }

            if (!isHostile && _targetAcquried)
            {
                SetOffAlert();
                ChangeToHostile();
            }
            else if (isAlert)
            {
                _alertTimer += Time.deltaTime;
                if (_alertTimer < _targetLostDelay)
                {
                    return;
                }
                else if (_returnToNeutralRoutine == null)
                {
                    _returnToNeutralRoutine = StartCoroutine(RetrunToNeutral(0));
                    return;
                }

            }
            else if (isHostile && !_targetAcquried && _returnToNeutralRoutine == null)
            {
                State = BazookaState.Alert;
                TargetStateChanged?.Invoke();
                _shooting.IsHostile = false;
                _returnToNeutralRoutine = StartCoroutine(RetrunToNeutral());
            }
        }

        public void SetOffAlert()
        {
            // TO DO: Rotate in direction of alert source
            if (State != BazookaState.Neutral) return;
            State = BazookaState.Alert;
            TargetStateChanged?.Invoke();
            _alertTimer = 0;
        }

        private void ChangeToHostile()
        {
            State = BazookaState.Hostile;
            TargetStateChanged?.Invoke();
            StartCoroutine(_shooting.CommenceHostilities());
        }

        private IEnumerator RetrunToNeutral(float delayMultiplicator = 1f)
        {
            while (_shooting.IsShooting)
            {
                yield return null;
            }

            yield return new WaitForSeconds(_targetLostDelay * delayMultiplicator);
            _returnToNeutralRoutine = null;
            State = BazookaState.Neutral;
            TargetStateChanged?.Invoke();
            _shooting.IsHostile = false;
        }

        bool IsInCone()
        {
            if (_target == null) return false;

            Vector3 directionToTarget = _target.position - _firePoint.transform.position;

            if (directionToTarget.sqrMagnitude > _coneRangeSquared) return false;

            float angleToTarget = Vector3.Angle(_firePoint.transform.forward, directionToTarget);
            return angleToTarget <= _coneAngle;
        }

        bool IsObstructed()
        {
            if (Physics.Raycast(_firePoint.transform.position, _target.position - _firePoint.transform.position, out RaycastHit hit, _coneRange, ~_ignoreLayers))
            {
                // Check if the ray hit a layer other than the target layer
                if ((1 << hit.collider.gameObject.layer & _targetLayer) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        void OnDrawGizmos()
        {
            // Draw the cone gizmo
            Gizmos.color = Color.yellow;
            DrawConeGizmo();

            if (_targetIsInCone)
            {
                // Draw the raycast gizmo
                Gizmos.color = Color.red;
                Gizmos.DrawLine(_firePoint.transform.position, _target.position);
            }
        }

        void DrawConeGizmo()
        {
            Vector3 basepoint = _firePoint.transform.position + _coneRange * _firePoint.transform.forward; // Adjust the length of the cone
            Vector3 vertex = _firePoint.transform.position;

            // Base circle defines a plane
            Vector3 planeNormal = _firePoint.transform.forward;

            // Base circle radius
            float radius = Mathf.Tan(Mathf.Deg2Rad * _coneAngle) * _coneRange;

            // Number of segments for the circle approximation
            int segments = 30;
            float angleIncrement = 360f / segments;

            Vector3 previousPoint = basepoint + Quaternion.AngleAxis(0, planeNormal) * (_firePoint.transform.right * radius);

            for (int i = 1; i <= segments; i++)
            {
                Vector3 nextPoint = basepoint + Quaternion.AngleAxis(angleIncrement * i, planeNormal) * (_firePoint.transform.right * radius);
                Gizmos.DrawLine(previousPoint, nextPoint);
                Gizmos.DrawLine(vertex, previousPoint);
                previousPoint = nextPoint;
            }

            // Draw the base point
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(vertex, basepoint);
        }

        private void OnValidate()
        {
            // Check if the variable has changed
            if (_coneRangeSquared != _coneRange * _coneRange)
            {
                _coneRange = Mathf.Sqrt(_coneRangeSquared);
            }
        }
    }
}
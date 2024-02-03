using System;
using UnityEngine;
using System.Collections;

namespace GGJ24
{
    public class Bazooka : MonoBehaviour
    {
        public Transform Target;

        [SerializeField] private GameObject _firePoint;
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
        private float _minHostileDuration = 3f;
        private float _hostileTimer = 0f;
        private bool _isHostile = false;

        [SerializeField] private BazookaState _state;
        public BazookaState State { get => _state; private set => _state = value; }
        private Shooting _shooting;

        public event Action TargetStateChanged;

        public enum BazookaState
        {
            Neutral = 0,
            Hostile = 1
        }

        private void Awake()
        {
            _coneRange = Mathf.Sqrt(_coneRangeSquared);
        }

        private void Start()
        {
            _shooting = _firePoint.GetComponent<Shooting>();
            _shooting.IsHostile = false;
            Target = GameObject.FindWithTag("PlayerTarget").transform;
        }

        void Update()
        {
            HandleTargeting();
        }

        private void HandleTargeting()
        {
            _isHostile = State == BazookaState.Hostile;
            _targetIsInCone = IsInCone();
            _targetAcquried = IsInCone() && !IsObstructed();

            if (!_isHostile && _targetAcquried)
            {
                ChangeToHostile();
            }

            if (_isHostile && !_targetAcquried)
            {
                _hostileTimer += Time.deltaTime;
                if (_hostileTimer > _minHostileDuration)
                    RetrunToNeutral();
            }
        }

        private void ChangeToHostile()
        {
            State = BazookaState.Hostile;
            TargetStateChanged?.Invoke();
            if (!_shooting.IsHostile) _shooting.CommenceHostilities();
        }

        private void RetrunToNeutral(float delayMultiplicator = 1f)
        {
            State = BazookaState.Neutral;
            TargetStateChanged?.Invoke();
            _shooting.IsHostile = false;
            _hostileTimer = 0f;
            //Debug.Log("return to neutral");
        }

        bool IsInCone()
        {
            if (Target == null) return false;

            Vector3 directionToTarget = Target.position - _firePoint.transform.position;

            if (directionToTarget.sqrMagnitude > _coneRangeSquared) return false;

            float angleToTarget = Vector3.Angle(_firePoint.transform.forward, directionToTarget);
            return angleToTarget <= _coneAngle;
        }

        bool IsObstructed()
        {
            if (Physics.Raycast(_firePoint.transform.position, Target.position - _firePoint.transform.position, out RaycastHit hit, _coneRange, ~_ignoreLayers))
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

            // Draw targeting gizmo
            Gizmos.color = _isHostile ? Color.red : _targetAcquried ? Color.cyan : Color.green;
            Gizmos.DrawLine(_firePoint.transform.position, Target.position);
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
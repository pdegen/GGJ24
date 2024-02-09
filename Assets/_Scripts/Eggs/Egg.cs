using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DamageNumbersPro;
using Unity.VisualScripting;

namespace GGJ24
{
    public class Egg : MonoBehaviour, ICollecetable, IDifficultydDependable
    {
        public static event Action CollectedEgg;
        public static event Action<Vector3> CollectedEggAtPosition;
        public string InteractionPrompt { get => "Collect"; }
        protected float _timeBonus;
        [SerializeField] protected GameObject _collectedEffect;

        [SerializeField] private LayerMask _defaultLayer;
        [SerializeField] private LayerMask _XRayLayer;
        private GameObject _meshObject;

        private void Awake()
        {
            _meshObject = gameObject.GetComponentInChildren<MeshRenderer>().gameObject;
        }

        protected virtual void Start()
        {
            _timeBonus = GameParamsLoader.BasicEggTimeBonus;
            InitEggMovement();

            // ensure AbilityManager is initialized by at least waiting until first egg collected
            //if (EggManager.CollectedEggs > 1 && AbilityManager.XRay.IsUnlocked) StartCoroutine(XRayRoutine(2f));
        }

        private IEnumerator XRayRoutine(float duration)
        {
            Debug.Log("st " + _meshObject.layer);
            _meshObject.layer = _XRayLayer;
            yield return new WaitForSeconds(duration);
            Debug.Log("end " + _meshObject.layer);
            _meshObject.layer = _defaultLayer;
        }

        protected virtual void InitEggMovement()
        {
            transform.DOMoveY(transform.position.y + 0.5f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }
        protected virtual void OnEnable()
        {
            GameManager.DifficultyChanged += RefreshDifficultyParams;
        }
        protected virtual void OnDisable()
        {
            GameManager.DifficultyChanged -= RefreshDifficultyParams;
            transform.DOKill();
        }

        public virtual void Collect(Collector collector)
        {
            CanvasManager.Instance.AddTimeBonus(_timeBonus);
            GameManager.Instance.RemainingTime += _timeBonus;
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.CollectionSFX, transform.position);
            EggManager.CollectedEggs++;
            CollectedEgg?.Invoke();
            CollectedEggAtPosition?.Invoke(transform.position);
            EggManager.Instance.SpawnEgg();
            GameObject collectedEffect = Instantiate(_collectedEffect, transform.position, Quaternion.identity);
            Destroy(collectedEffect, 2f);
            Destroy(gameObject);
        }

        public virtual void RefreshDifficultyParams()
        {
            _timeBonus = GameParamsLoader.BasicEggTimeBonus;
        }
    }
}



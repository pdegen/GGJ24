using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DamageNumbersPro;

namespace GGJ24
{
    public class Egg : MonoBehaviour, ICollecetable
    {
        public static event Action CollectedEgg;
        public static event Action<Vector3> CollectedEggAtPosition;
        public string InteractionPrompt { get => "Collect"; }
        protected float _timeBonus;

        protected virtual void Start()
        {
            _timeBonus = GameParamsLoader.BasicEggTimeBonus;
            InitEggMovement();
        }

        protected virtual void InitEggMovement()
        {
            transform.DOMoveY(transform.position.y + 0.5f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }

        protected virtual void OnDisable()
        {
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
            Destroy(gameObject);
        }
    }
}



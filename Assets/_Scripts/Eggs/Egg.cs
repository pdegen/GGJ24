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
        [SerializeField] private DamageNumber _timeBonusNumber;


        protected virtual void Start()
        {
            transform.DOMoveY(transform.position.y + 0.5f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }

        protected virtual void OnDisable()
        {
            transform.DOKill();
        }

        public virtual void Collect(Collector collector)
        {
            //_timeBonusNumber.Spawn(transform.position, GameParamsLoader.BasicEggTimeBonus);
            CanvasManager.Instance.AddTimeBonus(GameParamsLoader.BasicEggTimeBonus);
            GameManager.Instance.RemainingTime += GameParamsLoader.BasicEggTimeBonus;
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.CollectionSFX, transform.position);
            EggManager.CollectedEggs++;
            CollectedEgg?.Invoke();
            CollectedEggAtPosition?.Invoke(transform.position);
            EggManager.Instance.SpawnEgg();
            Destroy(gameObject);
        }
    }
}



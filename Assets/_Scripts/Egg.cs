using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GGJ24
{
    public class Egg : MonoBehaviour, ICollecetable
    {
        public static event Action CollectedEgg;
        public string InteractionPrompt { get => "Collect"; }


        private void Start()
        {
            transform.DOMoveY(transform.position.y + 0.5f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }

        private void OnDisable()
        {
            transform.DOKill();
        }

        public void Collect(Collector collector)
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.CollectionSFX, transform.position);
            EggSpawner.CollectedEggs++;
            CollectedEgg?.Invoke();
            EggSpawner.Instance.SpawnEgg();
            Destroy(gameObject);
        }
    }
}



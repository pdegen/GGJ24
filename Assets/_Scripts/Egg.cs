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
        //public static int TotalEggs = 0;
        public static int CollectedEggs = 0;
        public string InteractionPrompt { get => "Collect"; }

        //private void Awake()
        //{
        //    TotalEggs++;
        //}

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
            Debug.Log("Collected egg");
            CollectedEggs++;
            CollectedEgg?.Invoke();
            Destroy(gameObject);
            EggSpawner.Instance.SpawnEgg();
            //if (CollectedEggs.Equals(TotalEggs))
            //{
            //    GameManager.Victory();
            //}
        }
    }
}



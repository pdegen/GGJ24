using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ24
{
    public class Egg : MonoBehaviour, ICollecetable
    {
        public static event Action CollectedEgg;
        public static int TotalEggs = 0;
        public static int CollectedEggs = 0;
        public string InteractionPrompt { get => "Collect"; }

        private void Awake()
        {
            TotalEggs++;
        }

        public void Collect(Collector collector)
        {
            Debug.Log("Collected egg");
            CollectedEggs++;
            CollectedEgg?.Invoke();
            Destroy(gameObject);
            if (CollectedEggs.Equals(TotalEggs))
            {
                GameManager.Victory();
            }
        }
    }
}



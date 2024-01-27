using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ24
{
    public class Egg : MonoBehaviour, ICollecetable
    {
        public static event Action CollectedEgg;
        public string InteractionPrompt { get => "Collect"; }

        public void Collect(Collector collector)
        {
            Debug.Log("Collected egg");
            CollectedEgg?.Invoke();
            Destroy(gameObject);
        }
    }
}



using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ24
{
    public class GoldenEgg : Egg
    {
        [SerializeField] private float _healAmount = 200f;
        [SerializeField] private float _eggLifeTime = 10f;

        private float _timer = 0f;

        private void Awake()
        {
            _timer = _eggLifeTime;
        }
        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0 ) { Destroy(gameObject); }
        }

        public static Action<float> HealPlayer;
        public override void Collect(Collector collector)
        {
            base.Collect(collector);
            HealPlayer?.Invoke(_healAmount);
        }
    }
}
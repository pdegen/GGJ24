using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GGJ24
{
    public class GoldenEgg : Egg, IDifficultydDependable
    {
        private float _eggLifeTime;
        [SerializeField] private Material _radialTimerMaterial;
        private float _timer;

        // For now limit to one golden egg at a time because we have only one material for the radial timer
        public static bool GoldenEggExists;

        protected override void Start()
        {
            base.Start();
            _eggLifeTime = GameParamsLoader.GoldenEggLifetime;
            _timer = _eggLifeTime;
            _timeBonus = GameParamsLoader.GoldenEggAdditiveTimeBonus + GameParamsLoader.BasicEggTimeBonus;
            GoldenEggExists = true;
            _radialTimerMaterial.SetFloat("_FillPercent", 1);
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0 ) { 
                Destroy(gameObject);
                EggManager.Instance.SpawnEgg();
                GoldenEggExists = false;
            }

            _radialTimerMaterial.SetFloat("_FillPercent", _timer / _eggLifeTime);
        }

        public static Action<float> HealPlayer;
        public override void Collect(Collector collector)
        {
            base.Collect(collector);
            HealPlayer?.Invoke(GameParamsLoader.GoldenEggHealAmount);
            GoldenEggExists = false;
        }

        public override void RefreshDifficultyParams()
        {
            _timeBonus = GameParamsLoader.GoldenEggAdditiveTimeBonus + GameParamsLoader.BasicEggTimeBonus;
            _eggLifeTime = GameParamsLoader.GoldenEggLifetime;
            _timer = _eggLifeTime;
        }
    }
}
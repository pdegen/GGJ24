using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GGJ24
{
    public class Gate : MonoBehaviour
    {
        [SerializeField] private Transform _leftGate;
        [SerializeField] private Transform _rightGate;
        [SerializeField] private float _openingDuration = 10f;

        private void OnEnable()
        {
            Egg.CollectedEgg += OpenGate;
        }

        private void OnDisable()
        {
            Egg.CollectedEgg -= OpenGate;
        }

        public void OpenGate()
        {
            if (EggManager.CollectedEggs != GameParamsLoader.EggsCollectedToSpawnTank) { return; }

            Egg.CollectedEgg -= OpenGate;

            _leftGate.DOLocalRotate(new Vector3(0, 270, 0), _openingDuration);
            _rightGate.DOLocalRotate(new Vector3(0, -90, 0), _openingDuration);

            StartCoroutine(CloseGate());
        }

        private IEnumerator CloseGate()
        {
            yield return new WaitForSeconds(_openingDuration + 5f);
            _leftGate.DOLocalRotate(new Vector3(0, 180, 0), _openingDuration);
            _rightGate.DOLocalRotate(new Vector3(0, 0, 0), _openingDuration);
        }
    }
}
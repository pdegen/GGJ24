using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GGJ24
{
    public class SliderWithDelay : MonoBehaviour
    {
        private float _value;
        public float Value { get => _value; set { _value = value; UpdateSlider(value); } }
        public float MaxValue { get; set; }
        public float MinValue { get; set; }

        [SerializeField] private Slider _mainSlider;
        [SerializeField] private Slider _delayedSlider;
        [SerializeField] private float _delay = 1f;
        [SerializeField] private float _duration = 1f;
        private Coroutine _delayRoutine;

        public void Init(float initialValue, float maxValue, float minValue = 0)
        {
            if (maxValue <= minValue) Debug.LogError($"Slider max value invalid: {maxValue}");
            _mainSlider.maxValue = maxValue;
            _mainSlider.minValue = minValue;
            _mainSlider.value = initialValue;
            _delayedSlider.maxValue = maxValue;
            _delayedSlider.minValue = minValue;
            _delayedSlider.value = initialValue;
            MaxValue = maxValue;
            MinValue = minValue;
            Value = initialValue;
        }

        private void UpdateSlider(float newValue)
        {
            DOTween.To(() => _mainSlider.value, x => _mainSlider.value = x, newValue, _duration);

            if (_delayRoutine == null)
            {
                _delayRoutine = StartCoroutine(UpdateDelayedSlider(newValue));
            }
            else
            {
                StopCoroutine(_delayRoutine);
                _delayRoutine = StartCoroutine(UpdateDelayedSlider(newValue));
            }
        }

        private IEnumerator UpdateDelayedSlider(float newValue)
        {
            yield return new WaitForSeconds(_delay);
            DOTween.To(() => _delayedSlider.value, x => _delayedSlider.value = x, newValue, _duration);
        }

    }
}
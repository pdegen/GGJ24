using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

namespace GGJ24
{
    public class SelectableUIComponent : MonoBehaviour
    {

        private Vector3 _baseScale;

        private void OnEnable()
        {
            _baseScale = transform.localScale;
        }

        public void PlaySubmitted()
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UISubmtiSFX, transform.position);
        }

        public void PlaySelected()
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UISelectSFX, transform.position);
        }

        public void IncreaseScale(float newscale = 1.1f)
        {
            transform.localScale *= newscale;
        }

        public void ReturnToBaseScale()
        {
            transform.localScale = _baseScale;
        }
    }
}
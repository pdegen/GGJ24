using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GGJ24
{
    public class SelectableUIComponent : MonoBehaviour
    {
        public void PlaySubmitted()
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UISubmtiSFX, transform.position);
        }

        public void PlaySelected()
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UISelectSFX, transform.position);
        }
    }
}
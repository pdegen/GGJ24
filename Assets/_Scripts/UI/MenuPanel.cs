using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuPanel : MonoBehaviour
{
    [SerializeField] private GameObject _firstSelected;

    private void OnEnable()
    {
        var eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(_firstSelected, new BaseEventData(eventSystem));
    }
}

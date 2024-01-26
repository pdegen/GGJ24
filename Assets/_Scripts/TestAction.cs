using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ24
{
    public class TestAction : MonoBehaviour
    {
        private StarterAssetsInputActions _inputActions;
        private int _testAnimID;

        private void Awake()
        {
            _inputActions = new StarterAssetsInputActions();
            _inputActions.Player.Enable();
        }

        private void OnEnable()
        {
            _inputActions.Player.Interact.performed += PerformTest;
        }

        private void OnDisable()
        {
            _inputActions.Player.Interact.performed -= PerformTest;
        }

        protected virtual void AssignAnimationIDs()
        {
            _testAnimID = Animator.StringToHash("Test");
        }

        private void PerformTest(InputAction.CallbackContext context)
        {
            Debug.Log("test");
        }
    }
}
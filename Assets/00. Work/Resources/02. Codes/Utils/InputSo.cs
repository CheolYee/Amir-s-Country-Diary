using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _00._Work.Resources._02._Codes.Utils
{
    [CreateAssetMenu(fileName = "InputSo", menuName = "SO/Input", order = 0)]
    public class InputSo : ScriptableObject, Controls.IPlayerActions
    {
        private Controls _controls;
        
        public Vector2 MoveInput {get; private set;}
        public Vector2 MousePosition {get; private set;}
        public bool RunKeyPressed {get; private set;}
        public event Action OnSpaceKeyPressed;
        public event Action OnInteractionKeyPressed;
        public event Action OnInventory1KeyPressed;
        public event Action OnInventory2KeyPressed;
        public event Action OnInventory3KeyPressed;
        
        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new Controls();
                _controls.Player.SetCallbacks(this);
            }
            
            _controls.Player.Enable();
        }

        private void OnDisable()
        {
            _controls.Player.Disable();
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }

        public void OnSpace(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnSpaceKeyPressed?.Invoke();
            }
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            MousePosition = context.ReadValue<Vector2>();
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                RunKeyPressed = true;
            }
            else if (context.canceled)
            {
                RunKeyPressed = false;
            }
        }

        public void OnInteraction(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            OnInteractionKeyPressed?.Invoke();
        }

        public void OnInventory_1(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnInventory1KeyPressed?.Invoke();
            }
        }

        public void OnInventory_2(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnInventory2KeyPressed?.Invoke();
            }
        }

        public void OnInventory_3(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnInventory3KeyPressed?.Invoke();
            }
        }
    }
}
using System;
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
        public event Action OnAttackKeyPressed;
        public event Action OnJumpKeyPressed;
        
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

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnAttackKeyPressed?.Invoke();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnJumpKeyPressed?.Invoke();
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            MousePosition = context.ReadValue<Vector2>();
        }
    }
}
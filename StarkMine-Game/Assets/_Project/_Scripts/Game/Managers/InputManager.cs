using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project._Scripts.Game.Managers
{
    public class InputManager : PersistentSingleton<InputManager>
    {
        public EventHandler<OnDragCameraEventArgs> OnDragCamera;

        public class OnDragCameraEventArgs : EventArgs
        {
            public enum EventType
            {
                Start,
                Perform,
                Cancel,
            }

            public EventType Type;
            public Vector2 MousePosition;
        }

        public EventHandler<OnZoomCameraEventArgs> OnZoomCamera;
        public Vector2 MousePosition { get; private set; }
        public Vector2 MouseInWorldPosition => Camera.main.ScreenToWorldPoint(MousePosition);

        public class OnZoomCameraEventArgs : EventArgs
        {
            public float Value;
        }

        private InputSystem_Actions _playerInputActions;

        private void Start()
        {
            _playerInputActions = new InputSystem_Actions();
            _playerInputActions.Player.Enable();
            _playerInputActions.Player.DragCamera.started += DragCameraOnStarted;
            _playerInputActions.Player.DragCamera.canceled += DragCameraOnCanceled;
            _playerInputActions.Player.MousePosition.performed += MousePositionOnPerformed;
            _playerInputActions.Player.ZoomCamera.performed += ZoomCameraOnPerformed;
        }

        private void ZoomCameraOnPerformed(InputAction.CallbackContext obj)
        {
            OnZoomCamera?.Invoke(this, new OnZoomCameraEventArgs
            {
                Value = obj.ReadValue<float>()
            });
        }


        private void DragCameraOnStarted(InputAction.CallbackContext obj)
        {
            OnDragCamera?.Invoke(this, new OnDragCameraEventArgs()
            {
                Type = OnDragCameraEventArgs.EventType.Start,
                MousePosition = MousePosition
            });
        }
        
        private void DragCameraOnCanceled(InputAction.CallbackContext obj)
        {
            OnDragCamera?.Invoke(this, new OnDragCameraEventArgs()
            {
                Type = OnDragCameraEventArgs.EventType.Cancel,
                MousePosition = MousePosition
            });
        }

        private void MousePositionOnPerformed(InputAction.CallbackContext obj)
        {
            MousePosition = obj.ReadValue<Vector2>();
        }

        public void DisableGameInput()
        {
            _playerInputActions.Player.Disable();
        }

        private void OnDisable()
        {
            _playerInputActions.Player.Disable();
        }
    }
}
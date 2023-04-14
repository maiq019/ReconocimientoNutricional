using ITCL.VisionNutricional.Runtime.TouchScreen;
using UnityEngine;
using UnityEngine.InputSystem;
using WhateverDevs.Core.Runtime.Common;

namespace ITCL.VisionNutricional.Runtime.Camera
{
    public class TouchManager : Singleton<TouchManager>
    {
        private TouchControls TouchControls;

        public delegate void StartTouchEvent(Vector2 position, float time);

        public event StartTouchEvent OnStartTouch;
        
        public delegate void EndTouchEvent(Vector2 position, float time);

        public event EndTouchEvent OnEndTouch;

        private void Awake() => TouchControls = new TouchControls();

        private void OnEnable()
        {
            TouchControls.Enable();
            TouchControls.Touch.TouchPress.started += StartTouch;
            TouchControls.Touch.TouchPress.canceled += EndTouch;
        }
        
        private void OnDisable()
        {
            TouchControls.Disable();
            TouchControls.Touch.TouchPress.started -= StartTouch;
            TouchControls.Touch.TouchPress.canceled -= EndTouch;
        }

        private void StartTouch(InputAction.CallbackContext context)
        {
            Logger.Debug("Touch started" + TouchControls.Touch.TouchPosition.ReadValue<Vector2>());
            OnStartTouch?.Invoke(TouchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.startTime);
        }
        
        private void EndTouch(InputAction.CallbackContext context)
        {
            Logger.Debug("Touch ended" + TouchControls.Touch.TouchPosition.ReadValue<Vector2>());
            OnEndTouch?.Invoke(TouchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.startTime);
        }
    }
}

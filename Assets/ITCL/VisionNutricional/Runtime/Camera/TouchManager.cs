using ITCL.VisionNutricional.Runtime.TouchScreen;
using UnityEngine;
using UnityEngine.InputSystem;
using WhateverDevs.Core.Runtime.Common;

namespace ITCL.VisionNutricional.Runtime.Camera
{
    public class TouchManager : Singleton<TouchManager>
    {
        private TouchControls Controls;

        public delegate void StartTouchEvent(Vector2 position, float time);

        public event StartTouchEvent OnStartTouch;
        
        public delegate void EndTouchEvent(Vector2 position, float time);

        public event EndTouchEvent OnEndTouch;

        public delegate void StartZoomEvent(Vector2 primaryPosition, Vector2 secondaryPosition);

        public event StartZoomEvent OnStartZoom;

        public delegate void StopZoomEvent();

        public event StopZoomEvent OnStopZoom;

        private void Awake()
        {
            Controls = new TouchControls();
        }

        private void OnEnable()
        {
            Controls.Enable();
            Controls.Touch.PrimaryTouchPress.started += StartTouch;
            Controls.Touch.PrimaryTouchPress.canceled += EndTouch;
            
            Controls.Touch.SecondTouchPress.started += StartZoom;
            Controls.Touch.SecondTouchPress.canceled += StopZoom;
        }
        
        private void OnDisable()
        {
            Controls.Disable();
            Controls.Touch.PrimaryTouchPress.started -= StartTouch;
            Controls.Touch.PrimaryTouchPress.canceled -= EndTouch;
            
            Controls.Touch.SecondTouchPress.started -= StartZoom;
            Controls.Touch.SecondTouchPress.canceled -= StopZoom;
        }

        private void StartTouch(InputAction.CallbackContext context)
        {
            Logger.Debug("Touch started" + Controls.Touch.PrimaryPosition.ReadValue<Vector2>());
            OnStartTouch?.Invoke(Controls.Touch.PrimaryPosition.ReadValue<Vector2>(), (float)context.startTime);
        }
        
        private void EndTouch(InputAction.CallbackContext context)
        {
            Logger.Debug("Touch ended" + Controls.Touch.PrimaryPosition.ReadValue<Vector2>());
            OnEndTouch?.Invoke(Controls.Touch.PrimaryPosition.ReadValue<Vector2>(), (float)context.startTime);
        }

        private void StartZoom(InputAction.CallbackContext context)
        {
            Logger.Debug("TouchManager zoom started");
            OnStartZoom?.Invoke(Controls.Touch.PrimaryPosition.ReadValue<Vector2>(), Controls.Touch.SecondPosition.ReadValue<Vector2>());
        }

        private void StopZoom(InputAction.CallbackContext context)
        {
            Logger.Debug("TouchManager zoom stopped");
            OnStopZoom?.Invoke();
        }
    }
}

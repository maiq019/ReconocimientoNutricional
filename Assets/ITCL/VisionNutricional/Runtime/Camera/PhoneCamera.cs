using System.Collections;
using ITCL.VisionNutricional.Runtime.Initialization;
using ModestTree;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using Zenject;

namespace ITCL.VisionNutricional.Runtime.Camera
{
    [RequireComponent(typeof(RawImage))]
    public class PhoneCamera : WhateverBehaviour<PhoneCamera>
    {
        /// <summary>
        /// Reference to the scene manager.
        /// </summary>
        [Inject] private ISceneManager sceneManager;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject] private ILocalizer localizer;

        [SerializeField] private SceneReference MainMenuScene;

        private bool CamAvailable;

        private WebCamTexture BackCam;

        private RawImage Background;

        [SerializeField] private HidableUiElement UIHide;

        [SerializeField] private HidableUiElement ScreenshotButtonVertical;

        [SerializeField] private HidableUiElement ScreenshotButtonHorizontal;

        private TouchManager TouchManager;

        private IEnumerator MainMenuLoader;

        private void Awake()
        {
            MainMenuLoader = Loader.LoadSceneCoroutine(
                sceneManager, MainMenuScene, localizer["Common/Title"], localizer["Debug/LoadingMainMenu"]);
            Background = GetComponent<RawImage>();

            WebCamDevice[] devices = WebCamTexture.devices;

            //Check for some camera detected
            if (devices.IsEmpty())
            {
                Logger.Error("No camera detected");
                CamAvailable = false;
                return;
            }

            //Check for back camera
            foreach (WebCamDevice cam in devices)
            {
                Logger.Debug("Found camera " + cam.name + !cam.isFrontFacing);
                if (!cam.isFrontFacing) BackCam = new WebCamTexture(cam.name, Screen.width, Screen.height);
            }

            if (BackCam == null)
            {
                Logger.Error("Unable to find back camera");
                return;
            }

            StartCamera();

            CamAvailable = true;
        }

        private void OnEnable()
        {
            TouchManager.OnStartZoom += StartZoom;
            TouchManager.OnStopZoom += StopZoom;
        }

        private void Update()
        {
            if (!CamAvailable) return;

            float scaleY = BackCam.videoVerticallyMirrored ? -1 : 1;
            Background.rectTransform.localScale = new Vector3(1, scaleY, 1);

            switch (BackCam.videoRotationAngle)
            {
                case 90:
                case -90:
                    StartCoroutine(CameraRotation(true));
                    break;
                case 180:
                case -180:
                    StartCoroutine(CameraRotation(false));
                    break;
                default:
                    Background.rectTransform.sizeDelta = new Vector2(Screen.height, Screen.width);
                    Background.rectTransform.localEulerAngles = new Vector3(0, 0, -BackCam.videoRotationAngle);
                    break;
            }
            
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            Logger.Debug("Back button pressed");
            LoadMainMenu();
        }

        private IEnumerator CameraRotation(bool vertical)
        {
            if (vertical) //Hide horizontal button, rotate screen, show vertical button
            {
                ScreenshotButtonHorizontal.Show(false);
                yield return WaitAFrame;
                Background.rectTransform.sizeDelta = new Vector2(Screen.height, Screen.width);
                yield return WaitAFrame;
                ScreenshotButtonVertical.Show();
                Background.rectTransform.localEulerAngles = new Vector3(0, 0, -BackCam.videoRotationAngle);
            }
            else //Hide vertical button, rotate screen, show horizontal button
            {
                ScreenshotButtonVertical.Show(false);
                yield return WaitAFrame;
                Background.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
                yield return WaitAFrame;
                ScreenshotButtonHorizontal.Show();
                Background.rectTransform.localEulerAngles = new Vector3(0, 0, -BackCam.videoRotationAngle);
            }
        }

        private void StartCamera()
        {
            //Play the camera image on the scene background
            BackCam.Play();
            Background.texture = BackCam;
            UIHide.Show();
        }

        public void StopCamera(Texture2D capture)
        {
            BackCam.Stop();
            Background.texture = capture;
        }

        private void StartZoom(Vector2 primaryPosition, Vector2 secondaryPosition)
        {
            Logger.Debug("PhoneCamera zoom started");
            StartCoroutine(nameof(ZoomDetection));
        }

        private void StopZoom()
        {
            Logger.Debug("PhoneCamera zoom stopped");
            StopCoroutine(nameof(ZoomDetection));
        }

        private IEnumerator ZoomDetection(Vector2 primaryPosition, Vector2 secondaryPosition)
        {
            float previousDistance = 0;

            while (true)
            {
                float newDistance = Vector2.Distance(primaryPosition, secondaryPosition);

                if (newDistance > previousDistance) ZoomIn();
                else if (newDistance < previousDistance) ZoomOut();

                previousDistance = newDistance;
                yield return null;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private void ZoomIn()
        {
            BackCam.requestedHeight *= 11;
            BackCam.requestedHeight /= 10;
            BackCam.requestedWidth *= 11;
            BackCam.requestedWidth /= 10;
        }

        private void ZoomOut()
        {
            BackCam.requestedHeight *= 9;
            BackCam.requestedHeight /= 10;
            BackCam.requestedWidth *= 9;
            BackCam.requestedWidth /= 10;
        }

        private void LoadMainMenu()
        {
            CoroutineRunner.RunRoutine(MainMenuLoader);
        }
    }
}
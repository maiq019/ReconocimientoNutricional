using System.Collections;
using ITCL.VisionNutricional.Runtime.Initialization;
using ModestTree;
using UnityEngine;
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

        /// <summary>
        /// Reference to the main menu scene to go back.
        /// </summary>
        [SerializeField] private SceneReference MainMenuScene;

        /// <summary>
        /// Flag to check if there is a camera.
        /// </summary>
        private bool CamAvailable;

        /// <summary>
        /// Reference to the back camera image.
        /// </summary>
        private WebCamTexture BackCam;

        /// <summary>
        /// Reference to the canvas background image.
        /// </summary>
        private RawImage Background;

        /// <summary>
        /// Indicator whether the camera is vertical or horizontal.
        /// </summary>
        private bool CameraVertical;

        /// <summary>
        /// Hidable for all the UI components.
        /// </summary>
        [SerializeField] private HidableUiElement UIHide;

        /// <summary>
        /// Hidable for the vertical screenshot button.
        /// </summary>
        [SerializeField] private HidableUiElement ScreenshotButtonVertical;

        /// <summary>
        /// Hidable for the horizontal screenshot button.
        /// </summary>
        [SerializeField] private HidableUiElement ScreenshotButtonHorizontal;

        [SerializeField] private Button BackButtonVertical;

        private HidableUiElement BackButtonVerticalHid;

        private EasySubscribableButton BackButtonVerticalSus;

        /// <summary>
        /// Reference to the touch manager input.
        /// </summary>
        private TouchManager TouchManager;

        /// <summary>
        /// Reference to the coroutine to load the main menu scene.
        /// </summary>
        private IEnumerator MainMenuLoader;

        /// <summary>
        /// Flag to not load the menu twice.
        /// </summary>
        private bool IsMenuLoading;

        /// <summary>
        /// Sets the references and gets the camera.
        /// </summary>
        private void Awake()
        {
            MainMenuLoader = Loader.LoadSceneCoroutine(
                sceneManager, MainMenuScene, localizer["Common/Title"], localizer["Debug/LoadingMainMenu"]);
            IsMenuLoading = false;
            Background = GetComponent<RawImage>();

            BackButtonVerticalHid = BackButtonVertical.GetComponent<HidableUiElement>();
            BackButtonVerticalSus = BackButtonVertical.GetComponent<EasySubscribableButton>();

            //Gets the existing cameras.
            WebCamDevice[] devices = WebCamTexture.devices;

            //Check for some camera detected.
            if (devices.IsEmpty())
            {
                Logger.Error("No camera detected");
                CamAvailable = false;
                return;
            }

            //Check for back camera.
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
            //Starts the back camera found.
            StartCamera();
            //Camera flag changed.
            CamAvailable = true;
        }

        private void OnEnable()
        {
            TouchManager.OnStartZoom += StartZoom;
            TouchManager.OnStopZoom += StopZoom;

            BackButtonVerticalSus += BackToCam;
        }

        private void OnDisable()
        {
            BackCam.Stop();
        }

        /// <summary>
        /// Manages the rotation of the device relevant for the camera display.
        /// </summary>
        private void Update()
        {
            if (!CamAvailable) return;

            //Checks and fixes if the device is upside-down.
            float scaleY = IsCameraVerticallyMirrored() ? -1 : 1;
            Background.rectTransform.localScale = new Vector3(1, scaleY, 1);

            //Checks and adjust if the device is vertical or horizontal.
            switch (BackCam.videoRotationAngle)
            {
                case 90:
                case -90:
                    CameraVertical = true;
                    StartCoroutine(CameraRotation());
                    break;
                case 180:
                case -180:
                    CameraVertical = false;
                    StartCoroutine(CameraRotation());
                    break;
                default:
                    Background.rectTransform.sizeDelta = new Vector2(Screen.height, Screen.width);
                    Background.rectTransform.localEulerAngles = new Vector3(0, 0, -BackCam.videoRotationAngle);
                    break;
            }

            //Loads the main menu with the android back button.
            if (Input.GetKeyDown(KeyCode.Escape) && !IsMenuLoading)
            {
                IsMenuLoading = true;
                Logger.Debug("Back button pressed");
                CoroutineRunner.RunRoutine(MainMenuLoader);
            }
        }

        /// <summary>
        /// Adjust the display from the camera on the canvas texture.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CameraRotation()
        {
            if (CameraVertical) //Hide horizontal button, rotate screen, show vertical button
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

        /// <summary>
        /// Consult warp for the mirrored indicator from the camera.
        /// </summary>
        public bool IsCameraVerticallyMirrored()
        {
            return BackCam.videoVerticallyMirrored;
        }

        /// <summary>
        /// Consult warp for the vertical indicator of the camera.
        /// </summary>
        /// <returns></returns>
        public bool IsCameraVertical()
        {
            return CameraVertical;
        }

        /// <summary>
        /// Starts the camera recording and its necessaries coroutines.
        /// </summary>
        private void StartCamera()
        {
            //Play the camera image on the scene background
            BackCam.Play();
            ShowCamera();
        }

        /// <summary>
        /// Display the camera images on the canvas' background texture
        /// </summary>
        private void ShowCamera()
        {
            Background.texture = BackCam;
            UIHide.Show();
            BackButtonVerticalHid.Show(false);
        }

        /// <summary>
        /// Stops the camera recording and sets the capture taken on the background texture.
        /// </summary>
        /// <param name="capture"></param>
        public void StopCamera(Texture2D capture)
        {
            Background.texture = capture;
            BackButtonVerticalHid.Show();
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

        private void BackToCam()
        {
            ShowCamera();
        }
    }
}
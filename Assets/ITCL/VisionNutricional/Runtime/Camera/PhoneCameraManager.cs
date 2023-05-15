using System.Collections;
using System.Collections.Generic;
using ITCL.VisionNutricional.Runtime.Initialization;
using ModestTree;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.Localization.Runtime.Ui;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using Zenject;

namespace ITCL.VisionNutricional.Runtime.Camera
{
    [RequireComponent(typeof(RawImage))]
    public class PhoneCameraManager : WhateverBehaviour<PhoneCameraManager>
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
        /// Reference to the cloud vision api manager object.
        /// </summary>
        [SerializeField] private GameObject CloudApiManager;
        
        /// <summary>
        /// Reference to the cloud vision api script.
        /// </summary>
        private CamTextureToCloudVision CloudApi;

        /// <summary>
        /// Reference to the cloud receiver script.
        /// </summary>
        private CloudReceiver CloudRec;

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
        [SerializeField] private RawImage Background;

        /// <summary>
        /// Indicator whether the camera is vertical or horizontal.
        /// </summary>
        private bool CameraVertical;

        /// <summary>
        /// Indicator whether the camera view is inverted or not.
        /// </summary>
        private bool CameraInverted;

        /// <summary>
        /// Hidable for all the UI components.
        /// </summary>
        [SerializeField] private HidableUiElement UIHide;

        /// <summary>
        /// Hidable for the vertical screenshot button.
        /// </summary>
        [SerializeField] private HidableUiElement ScreenshotButtonVertical;

        private ScreenShotButton ScreenShotButtonComponent;

        /// <summary>
        /// Hidable for the horizontal screenshot button.
        /// </summary>
        [SerializeField] private HidableUiElement ScreenshotButtonHorizontal;

        /// <summary>
        /// Flag to know when a screenshot is being captured.
        /// </summary>
        [HideInInspector] public bool TakingScreenshot;

        /// <summary>
        /// Reference to the back button.
        /// </summary>
        [SerializeField] private Button BackButton;

        /// <summary>
        /// Subscribable for the back button.
        /// </summary>
        private EasySubscribableButton BackButtonSus;

        /// <summary>
        /// Reference to the send button.
        /// </summary>
        [SerializeField] private Button SendButton;

        /// <summary>
        /// Hidable for the send button.
        /// </summary>
        private HidableUiElement SendButtonHid;
        
        /// <summary>
        /// Subscribable for the send button.
        /// </summary>
        private EasySubscribableButton SendButtonSus;

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
            CloudApi = CloudApiManager.GetComponent<CamTextureToCloudVision>();
            CloudRec = CloudApiManager.GetComponent<CloudReceiver>();


            MainMenuLoader = Loader.LoadSceneCoroutine(
                sceneManager, MainMenuScene, localizer["Common/Title"], localizer["Debug/LoadingMainMenu"]);

            IsMenuLoading = false;

            ScreenShotButtonComponent = ScreenshotButtonVertical.GetComponent<ScreenShotButton>();
            BackButtonSus = BackButton.GetComponent<EasySubscribableButton>();
            SendButtonHid = SendButton.GetComponent<HidableUiElement>();
            SendButtonSus = SendButton.GetComponent<EasySubscribableButton>();

            //Gets the existing cameras.
            WebCamDevice[] devices = WebCamTexture.devices;

            //Check for some camera detected.
            if (devices.IsEmpty())
            {
                Log.Error("No camera detected");
                CamAvailable = false;
                return;
            }

            //Check for back camera.
            foreach (WebCamDevice cam in devices)
            {
                Log.Debug("Found camera " + cam.name + !cam.isFrontFacing);
                if (!cam.isFrontFacing) BackCam = new WebCamTexture(cam.name, Screen.width, Screen.height);
            }

            if (BackCam == null)
            {
                Log.Error("Unable to find back camera");
                return;
            }

            //Starts the back camera found.
            StartCamera();
            //Camera flag changed.
            CamAvailable = true;
        }

        private void OnEnable()
        {
            //TouchManager.OnStartZoom += StartZoom;
            //TouchManager.OnStopZoom += StopZoom;

            BackButtonSus += BackButtonPress;
            //SendButtonSus += SendImageToCloudVision;
            SendButtonSus += SendImageFake;
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
            //Loads the main menu with the android back button.
            if (Input.GetKeyDown(KeyCode.Escape) && !IsMenuLoading)
            {
                IsMenuLoading = true;
                Log.Debug("Android back button pressed");
                CoroutineRunner.RunRoutine(MainMenuLoader);
            }
            
            //Checks if there is a camera, if it is playing and if there is not a screenshot being taken.
            if (!CamAvailable || !BackCam.isPlaying || TakingScreenshot) return;

            //Checks and fixes if the device is upside-down.
            CameraInverted = BackCam.videoVerticallyMirrored;
            Background.rectTransform.localScale = IsCameraVerticallyMirrored()
                ? new Vector3(1, -1, 1)
                : new Vector3(1, 1, 1);

            //Checks and adjust if the device is vertical or horizontal.
            switch (BackCam.videoRotationAngle)
            {
                case 90:
                case -90:
                    CameraVertical = true;
                    ShowScreenshotButtonVertical();
                    Background.rectTransform.sizeDelta = new Vector2(Screen.height, Screen.width);
                    break;
                case 0:
                    CameraVertical = false;
                    ShowScreenshotButtonHorizontal();
                    Background.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
                    break;
                case 180:
                case -180:
                    CameraVertical = false;
                    CameraInverted = true;
                    ShowScreenshotButtonHorizontal();
                    Background.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
                    break;
                default:
                    Background.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
                    break;
            }

            Background.rectTransform.localEulerAngles = new Vector3(0, 0, -BackCam.videoRotationAngle);
        }

        /// <summary>
        /// Consult warp for the mirrored indicator from the camera.
        /// </summary>
        private bool IsCameraVerticallyMirrored()
        {
            return CameraInverted;
        }

        /// <summary>
        /// Consult warp for the vertical indicator of the camera.
        /// </summary>
        /// <returns></returns>
        private bool IsCameraVertical()
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
            Background.texture = BackCam;
        }

        /// <summary>
        /// Stops the camera recording and sets the capture taken on the background texture.
        /// </summary>
        /// <param name="capture"></param>
        public void StopCamera(Texture2D capture)
        {
            BackCam.Stop();
            if (IsCameraVerticallyMirrored())
                Background.rectTransform.localScale = IsCameraVertical() ? new Vector3(1, -1, 1) : new Vector3(-1, -1, 1);
            else Background.rectTransform.localScale = new Vector3(1, 1, 1);

            Background.rectTransform.localEulerAngles = IsCameraVertical()
                ? new Vector3(0, 0, -BackCam.videoRotationAngle + 90)
                : new Vector3(0, 0, -BackCam.videoRotationAngle);
            Background.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
            Background.texture = capture;
            ShowSendButton();
        }

        private void ShowScreenshotButtonHorizontal(bool show = true)
        {
            UIHide.Show();
            ScreenshotButtonVertical.Show(!show);
            ScreenshotButtonHorizontal.Show(show);
            SendButtonHid.Show(!show);
        }

        private void ShowScreenshotButtonVertical(bool show = true)
        {
            UIHide.Show();
            ScreenshotButtonVertical.Show(show);
            ScreenshotButtonHorizontal.Show(!show);
            SendButtonHid.Show(!show);
        }

        private void ShowSendButton(bool show = true)
        {
            UIHide.Show();
            ScreenshotButtonVertical.Show(!show);
            ScreenshotButtonHorizontal.Show(!show);
            SendButtonHid.Show(show);
        }
        
        private void BackButtonPress()
        {
            if (BackCam.isPlaying)
            {
                BackButtonSus -= BackButtonPress;
                IsMenuLoading = true;
                CoroutineRunner.RunRoutine(MainMenuLoader);
            }
            else
            {
                StartCamera();
            }
        }

        private void SendImageToCloudVision()
        {
            CloudApi.SendImageToCloudVision(ScreenShotButtonComponent.screenshot);
        }

        private void SendImageFake()
        {
            CamTextureToCloudVision.Vertex vertice1 = new() { x=0f,y=0.590967f };
            CamTextureToCloudVision.Vertex vertice2 = new() { x=0.34507558f,y=0.590967f };
            CamTextureToCloudVision.Vertex vertice3 = new() { x=0.34507558f,y=0.8935118f };
            CamTextureToCloudVision.Vertex vertice4 = new() { x=0f,y=0.8935118f };

            List<CamTextureToCloudVision.Vertex> vertexList = new List<CamTextureToCloudVision.Vertex>() { vertice1, vertice2, vertice3, vertice4 };
            
            CloudRec.DrawObject(vertexList);
        }

        

        /*
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
        */
    }
}
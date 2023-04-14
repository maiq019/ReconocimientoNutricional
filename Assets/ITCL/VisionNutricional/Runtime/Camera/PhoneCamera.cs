using ModestTree;
using UnityEngine;
using UnityEngine.UI;
using WhateverDevs.Core.Behaviours;

namespace ITCL.VisionNutricional.Runtime.Camera
{
    [RequireComponent(typeof(RawImage))]
    public class PhoneCamera : WhateverBehaviour<PhoneCamera>
    {
        private bool camAvailable;

        private WebCamTexture backCam;

        [HideInInspector]
        public RawImage Background;

        private void Awake()
        {
            Background = GetComponent<RawImage>();

            WebCamDevice[] devices = WebCamTexture.devices;

            //Check for some camera detected
            if (devices.IsEmpty())
            {
                Logger.Error("No camera detected");
                camAvailable = false;
                return;
            }

            //Check for back camera
            foreach (WebCamDevice cam in devices)
            {
                Logger.Debug("Found camera " + cam.name + !cam.isFrontFacing);
                if (!cam.isFrontFacing) backCam = new WebCamTexture(cam.name, Screen.width, Screen.height);
            }

            if (backCam == null)
            {
                Logger.Error("Unable to find back camera");
                return;
            }
            
            StartCamera();
            
            camAvailable = true;
        }

        private void Update()
        {
            if (!camAvailable) return;

            //float ratio = (float)backCam.width / backCam.height;
            //fit.aspectRatio = ratio;

            float scaleY = backCam.videoVerticallyMirrored ? -1 : 1;
            Background.rectTransform.localScale = new Vector3(1, scaleY, 1);

            switch (backCam.videoRotationAngle)
            {
                case 90:
                case -90:
                    Background.rectTransform.sizeDelta = new Vector2(Screen.height, Screen.width);
                    break;
                case 180:
                case -180:
                    Background.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
                    /*fit.aspectMode = AspectRatioFitter.AspectMode.None;
                    Background.rectTransform.offsetMax = new Vector2(0, 0);
                    Background.rectTransform.offsetMin = new Vector2(0, 0);*/
                    break;
                default:
                    Background.rectTransform.sizeDelta = new Vector2(Screen.height, Screen.width);
                    //fit.aspectMode = fit.aspectMode;
                    break;
            }

            Background.rectTransform.localEulerAngles = new Vector3(0, 0, -backCam.videoRotationAngle);
            
        }

        public void StartCamera()
        {
            //Play the camera image on the scene background
            backCam.Play();
            Background.texture = backCam;
        }

        public void StopCamera()
        {
            backCam.Stop();
        }
    }
}

using System;
using ModestTree;
using UnityEngine;
using UnityEngine.UI;
using WhateverDevs.Core.Behaviours;

namespace ITCL.VisionNutricional.Runtime.Camera
{
    public class PhoneCamera : WhateverBehaviour<PhoneCamera>
    {
        private bool camAvailable;

        private WebCamTexture backCam;

        private Texture defaultBackground;

        [SerializeField]
        private RawImage Background;

        private AspectRatioFitter fit;

        private void OnEnable()
        {
            fit = Background.GetComponent<AspectRatioFitter>();
            
            defaultBackground = Background.texture;
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
                if (!cam.isFrontFacing) backCam = new WebCamTexture(cam.name, Screen.width, Screen.height);
            }

            if (backCam == null)
            {
                Logger.Error("Unable to find back camera");
                return;
            }
            
            camAvailable = true;
            
            //Play the camera image on the scene background
            backCam.Play();
            Background.texture = backCam;
        }

        private void Update()
        {
            if (!camAvailable) return;

            float ratio = (float)backCam.width / (float)backCam.height;
            fit.aspectRatio = ratio;

            float scaleY = backCam.videoVerticallyMirrored ? -1 : 1;
            Background.rectTransform.localScale = new Vector3(1, scaleY, 1);

            Background.rectTransform.localEulerAngles = new Vector3(0, 0, -backCam.videoRotationAngle);
        }
    }
}

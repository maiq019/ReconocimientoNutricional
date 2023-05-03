using UnityEngine;
using WhateverDevs.Core.Runtime.Ui;

namespace ITCL.VisionNutricional.Runtime.Camera
{
    public class CloudVisionSendButton : ActionOnButtonClick<CloudVisionSendButton>
    {
        [SerializeField] private CamTextureToCloudVision CloudApi;
        
        protected internal Texture2D capture;

        protected override void ButtonClicked()
        {
            CloudApi.SendImageToCloudVision(capture);
        }
    }
}

using System;
using System.Collections;
using UnityEngine;
using WhateverDevs.Core.Runtime.Ui;

namespace ITCL.VisionNutricional.Runtime.Camera
{
    public class ScreenShot : ActionOnButtonClick<ScreenShot>
    {
        [SerializeField] private HidableUiElement UIHide;

        [SerializeField] private PhoneCamera Camera;

        

        protected override void ButtonClicked() => TakeScreenshot();

        private void TakeScreenshot() => StartCoroutine(nameof(ScreenshotCoroutine));

        private IEnumerator ScreenshotCoroutine()
        {
            //Begin screenshot process.
            Camera.TakingScreenshot = true;
            //Hide UI elements to not appear in the screenshot.
            UIHide.Show(false);

            yield return new WaitForEndOfFrame();
            //Creates and applies the screenshot texture.
            Texture2D capture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            capture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            capture.Apply();
            Logger.Debug("Capture texture created");
            //Stops the camera.
            Camera.StopCamera(capture);
            //Finish the screenshot process.
            Camera.TakingScreenshot = false;
            
            yield return new WaitForEndOfFrame();
            //Save the screenshot
            string screenshotName = "Screenshot_Nutrision_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
            NativeGallery.Permission per = NativeGallery.SaveImageToGallery(capture, "Nutrision", screenshotName);
            switch (per)
            {
                case NativeGallery.Permission.Denied:
                    Logger.Error("Gallery permission denied");
                    break;
                case NativeGallery.Permission.Granted:
                    Logger.Debug("Gallery permission granted, image saved");
                    break;
                case NativeGallery.Permission.ShouldAsk:
                    Logger.Debug("Gallery permission should ask");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //Destroy(texture);
            //Logger.Debug("Texture destroyed");

            yield return new WaitForEndOfFrame();
        }
    }
}
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

        public void TakeScreenshot()
        {
            UIHide.Show(false);
            Logger.Debug("UI hide");
            StartCoroutine(nameof(ScreenshotCoroutine));
            UIHide.Show();
        }

        private IEnumerator ScreenshotCoroutine()
        {
            yield return new WaitForEndOfFrame();

            Texture2D capture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            capture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            capture.Apply();
            Logger.Debug("Capture texture created");

            Camera.StopCamera(capture);

            yield return new WaitForEndOfFrame();

            //Save the screenshot
            string screenshotName = "Screenshot_Nutrision_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
            NativeGallery.Permission per = NativeGallery.SaveImageToGallery(capture, "Nutrision", screenshotName);
            if (per == NativeGallery.Permission.Denied)
            {
                Logger.Error("Gallery permission denied");
            }
            else if (per == NativeGallery.Permission.Granted)
            {
                Logger.Debug("Gallery permission granted, image saved");
            }
            else
            {
                Logger.Debug("Gallery permission should ask");
            }

            //Destroy(texture);
            //Logger.Debug("Texture destroyed");

            yield return new WaitForEndOfFrame();
        }
    }
}
using System.Collections;
using UnityEngine;
using WhateverDevs.Core.Runtime.Ui;

namespace ITCL.VisionNutricional.Runtime.Camera
{
    public class ScreenShot : ActionOnButtonClick<ScreenShot>
    {
        [SerializeField]
        private HidableUiElement UIHide;

        private HidableUiElement ScreenShotButtonHide;

        [SerializeField]
        private PhoneCamera Camera;

        private void Awake()
        {
            ScreenShotButtonHide = GetComponent<HidableUiElement>();
        }

        protected override void ButtonClicked() => TakeScreenshot();

        public void TakeScreenshot()
        {
            UIHide.Show(false);
            ScreenShotButtonHide.Show(false);
            Logger.Debug("UI hide");
            StartCoroutine(nameof(ScreenshotCoroutine));
            UIHide.Show();
            Logger.Debug("UI shown");
        }

        private IEnumerator ScreenshotCoroutine()
        {
            yield return new WaitForEndOfFrame();

            Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            texture.Apply();
            Logger.Debug("Texture created");

            //Save the screenshot
            string screenshotName = "Screenshot_Nutrision_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
            NativeGallery.SaveImageToGallery(texture, "Nutrision", screenshotName);
            Logger.Debug("Image saved");

            Camera.Background.texture = texture;
            
            Camera.StopCamera();   

            //Destroy(texture);
            //Logger.Debug("Texture destroyed");

            yield return new WaitForEndOfFrame();
        }
    }
}

using System.Collections;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Ui;

namespace ITCL.VisionNutricional.Runtime.Camera
{
    public class ScreenShot : WhateverBehaviour<ScreenShot>
    {
        [SerializeField]
        private HidableUiElement UIHide;
        
        public void TakeScreenshot()
        {
            UIHide.Show(false);
            StartCoroutine(nameof(ScreenshotCoroutine));
            UIHide.Show();
        }

        private IEnumerator ScreenshotCoroutine()
        {
            yield return new WaitForEndOfFrame();

            Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            texture.Apply();

            //Save the screenshot
            string screenshotName = "Screenshot_Nutrision_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";

            NativeGallery.SaveImageToGallery(texture, "Nutrision", screenshotName);
            
            Destroy(texture);
        }
    }
}

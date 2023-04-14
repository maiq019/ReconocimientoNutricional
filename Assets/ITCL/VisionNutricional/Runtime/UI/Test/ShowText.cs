using System.Collections;
using UnityEngine;
using WhateverDevs.Core.Runtime.Ui;

namespace ITCL.VisionNutricional.Runtime.UI.Test
{
    public class ShowText : ActionOnButtonClick<ShowText>
    {
        [SerializeField]
        private HidableUiElement TextHide;

        protected override void ButtonClicked()
        {
            StartCoroutine(nameof(ShowTextCoroutine));
        }

        private IEnumerator ShowTextCoroutine()
        {
            TextHide.Show();
            yield return new WaitForSeconds(2);
            TextHide.Show(false);
        }
    }
}

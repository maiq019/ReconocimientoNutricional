using System;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Ui;

namespace ITCL.VisionNutricional.Runtime.UI
{
    public class MainMenuManager : WhateverBehaviour<MainMenuManager>
    {
        /// <summary>
        /// Config popup window
        /// </summary>
        [SerializeField] 
        private HidableUiElement ConfigScreenHide;

        /// <summary>
        /// Config ok button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton ConfigOkButtonSus;

        /// <summary>
        /// Subscribes to the OnButtonClicked of the corresponding button.
        /// </summary>
        private void OnEnable()
        {
            HideConfigScreen();

            ConfigOkButtonSus += HideConfigScreen;
        }

        private void HideConfigScreen() => ConfigScreenHide.Show(false);


    }
}

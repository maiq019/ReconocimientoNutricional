using TMPro;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Build;
using WhateverDevs.Core.Runtime.Ui;
using Version = WhateverDevs.Core.Runtime.Build.Version;

namespace ITCL.VisionNutricional.Runtime.UI
{
    /// <summary>
    /// Window popup manager for Main Menu.
    /// </summary>
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
        [SerializeField] private EasySubscribableButton ConfigButtonSus;

        /// <summary>
        /// Config ok button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton ConfigOkButtonSus;

        /// <summary>
        /// Reference to the version control scriptable.
        /// </summary>
        [SerializeField] private Version Version;
        
        /// <summary>
        /// Version text on the bottom of the main menu window.
        /// </summary>
        [SerializeField] private TMP_Text VersionText;

        /// <summary>
        /// Subscribes to the OnButtonClicked of the corresponding button.
        /// </summary>
        private void OnEnable()
        {
            HideConfigScreen();
            ConfigButtonSus += ShowConfigScreen;
            ConfigOkButtonSus += HideConfigScreen;

            VersionText.text += Version.ToString(VersionDisplayMode.Short);
        }
        
        /// <summary>
        /// Shows the config popup window.
        /// </summary>
        private void ShowConfigScreen() => ConfigScreenHide.Show();

        /// <summary>
        /// Hides the config popup window.
        /// </summary>
        private void HideConfigScreen() => ConfigScreenHide.Show(false);


    }
}

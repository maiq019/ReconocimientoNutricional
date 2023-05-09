using ITCL.VisionNutricional.Runtime.Initialization;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using Zenject;

namespace ITCL.VisionNutricional.Runtime.MainMenu
{
    public class ConfigManager : WhateverBehaviour<ConfigManager>
    {
        /// <summary>
        /// Reference to the scene manager.
        /// </summary>
        [Inject] private ISceneManager sceneManager;
        
        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject] private ILocalizer localizer;
        
        /// <summary>
        /// Config popup window
        /// </summary>
        [SerializeField] private HidableUiElement ConfigScreenHide;

        /// <summary>
        /// Config button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton ConfigButtonSus;

        /// <summary>
        /// Config ok button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton ConfigOkButtonSus;
        
        /// <summary>
        /// Logout button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton LogoutButtonSus;

        /// <summary>
        /// Reference to the Login screen.
        /// </summary>
        [SerializeField] private SceneReference LoginScene;
        
        /// <summary>
        /// Subscribes to the OnButtonClicked of the corresponding button.
        /// </summary>
        private void OnEnable()
        {
            ConfigButtonSus += ShowConfigScreen;
            ConfigOkButtonSus += HideConfigScreen;
            
            LogoutButtonSus += Logout;
        }
        
        /// <summary>
        /// Logs out from the user account and returns to the login screen.
        /// </summary>
        private void Logout()
        {
            LogoutButtonSus -= Logout;
            CoroutineRunner.RunRoutine(
                Loader.LoadSceneCoroutine(sceneManager, LoginScene, localizer["Common/title"], localizer["Common/Menu/Logout"]));
        }
        
        /// <summary>
        /// Shows the config popup window.
        /// </summary>
        private void ShowConfigScreen() => ConfigScreenHide.Show();

        /// <summary>
        /// Hides the config popup window.
        /// </summary>
        public void HideConfigScreen() => ConfigScreenHide.Show(false);
    }
}

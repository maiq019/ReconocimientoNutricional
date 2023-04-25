using ITCL.VisionNutricional.Runtime.Initialization;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using Zenject;

namespace ITCL.VisionNutricional.Runtime.UI
{
    /// <summary>
    /// Window popup manager for Main Menu.
    /// </summary>
    public class MainMenuManager : WhateverBehaviour<MainMenuManager>
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

        [SerializeField] private SceneReference LoginScene;

        /// <summary>
        /// Scan for food button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton ScanButtonSus;

        /// <summary>
        /// Reference to the camera scene to load.
        /// </summary>
        [SerializeField] private SceneReference CameraScene;

        /// <summary>
        /// Subscribes to the OnButtonClicked of the corresponding button.
        /// </summary>
        private void OnEnable()
        {
            HideConfigScreen();
            ConfigButtonSus += ShowConfigScreen;
            ConfigOkButtonSus += HideConfigScreen;
            
            LogoutButtonSus += Logout;
            ScanButtonSus += LoadCameraScene;
        }

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
        private void HideConfigScreen() => ConfigScreenHide.Show(false);

        /// <summary>
        /// Loads the camera scene.
        /// </summary>
        private void LoadCameraScene()
        {
            ScanButtonSus -= LoadCameraScene;
            CoroutineRunner.RunRoutine(Loader.LoadSceneCoroutine(
                sceneManager, CameraScene, localizer["Common/Title"], localizer["Debug/LoadingCamera"]));
        }
    }
}
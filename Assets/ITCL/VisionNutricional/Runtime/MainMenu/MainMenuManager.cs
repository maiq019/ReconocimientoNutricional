using ITCL.VisionNutricional.Runtime.Initialization;
using UnityEngine;
using UnityEngine.Serialization;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using Zenject;

namespace ITCL.VisionNutricional.Runtime.MainMenu
{
    /// <summary>
    /// Manager for Main Menu and its screens.
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
        /// Reference to the config popup screen.
        /// </summary>
        [SerializeField] private ConfigManager ConfigWindow;

        /// <summary>
        /// Scan for food button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton ScanButtonSus;

        /// <summary>
        /// Reference to the camera scene to load.
        /// </summary>
        [SerializeField] private SceneReference CameraScene;
        
        /// <summary>
        /// Scan for food button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton HistoricButtonSus;

        /// <summary>
        /// Reference to the camera scene to load.
        /// </summary>
        [SerializeField] private SceneReference HistoricScene;

        /// <summary>
        /// Subscribes to the OnButtonClicked of the corresponding button.
        /// </summary>
        private void OnEnable()
        { 
            ConfigWindow.HideConfigScreen();
            ScanButtonSus += LoadCameraScene;
            HistoricButtonSus += LoadHistoricScene;
        }

        /// <summary>
        /// Loads the camera scene.
        /// </summary>
        private void LoadCameraScene()
        {
            ScanButtonSus -= LoadCameraScene;
            CoroutineRunner.RunRoutine(Loader.LoadSceneCoroutine(
                sceneManager, CameraScene, localizer["Common/Title"], localizer["Debug/LoadingCamera"]));
        }
        
        /// <summary>
        /// Loads the camera scene.
        /// </summary>
        private void LoadHistoricScene()
        {
            HistoricButtonSus -= LoadHistoricScene;
            CoroutineRunner.RunRoutine(Loader.LoadSceneCoroutine(
                sceneManager, HistoricScene, localizer["Common/Title"], localizer["Debug/LoadingHistoric"]));
        }
    }
}
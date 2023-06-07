using ITCL.VisionNutricional.Runtime.Initialization;
using ITCL.VisionNutricional.Runtime.Login;
using UnityEngine;
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
        /// Reference to this scene.
        /// </summary>
        [SerializeField] private SceneReference ThisScene;

        /// <summary>
        /// Reference to the config button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton ConfigButtonSus;
        
        /// <summary>
        /// Reference to the config popup screen.
        /// </summary>
        [SerializeField] private SceneReference ConfigScene;
        
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
            ConfigButtonSus += LoadConfigScene;
            ScanButtonSus += LoadCameraScene;
            HistoricButtonSus += LoadHistoricScene;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
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
        /// Loads the historic scene.
        /// </summary>
        private void LoadHistoricScene()
        {
            HistoricButtonSus -= LoadHistoricScene;
            CoroutineRunner.RunRoutine(Loader.LoadSceneCoroutine(
                sceneManager, HistoricScene, localizer["Common/Title"], localizer["Debug/LoadingHistoric"]));
        }
        
        /// <summary>
        /// Loads the config scene.
        /// </summary>
        private void LoadConfigScene()
        {
            ConfigButtonSus -= LoadConfigScene;
            Session.PreviousScene = ThisScene;
            CoroutineRunner.RunRoutine(Loader.LoadSceneCoroutine(
                sceneManager, ConfigScene, localizer["Common/Title"], localizer["Debug/LoadingConfig"]));
        }
    }
}
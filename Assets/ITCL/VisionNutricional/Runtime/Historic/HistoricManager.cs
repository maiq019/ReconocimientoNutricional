using ITCL.VisionNutricional.Runtime.Initialization;
using ITCL.VisionNutricional.Runtime.MainMenu;
using UnityEngine;
using UnityEngine.UI;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using Zenject;

namespace ITCL.VisionNutricional.Runtime.Historic
{
    /// <summary>
    /// Manager for Historic and its screens.
    /// </summary>
    public class HistoricManager : WhateverBehaviour<HistoricManager>
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
        /// Main button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton MainButtonSus;
        
        /// <summary>
        /// Reference to the next scene to load.
        /// </summary>
        [SerializeField] private SceneReference MainMenuScene;
        
        /// <summary>
        /// Reference to the config popup screen.
        /// </summary>
        [SerializeField] private ConfigManager configWindow;

        
        
        /// <summary>
        /// Subscribes to the OnButtonClicked of the corresponding button.
        /// </summary>
        private void OnEnable()
        {
            configWindow.HideConfigScreen();
            MainButtonSus += LoadMainMenu;
            
        }
        
        /// <summary>
        /// Loads the camera scene.
        /// </summary>
        private void LoadMainMenu()
        {
            MainButtonSus -= LoadMainMenu;
            CoroutineRunner.RunRoutine(Loader.LoadSceneCoroutine(
                sceneManager, MainMenuScene, localizer["Common/Title"], localizer["Debug/LoadingMainMenu"]));
        }
    }
}

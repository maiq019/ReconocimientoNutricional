using System.Collections;
using TMPro;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Build;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using Zenject;
using Version = WhateverDevs.Core.Runtime.Build.Version;

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
        [Inject]
        private ISceneManager sceneManager;
        
        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;
        
        /// <summary>
        /// Config popup window
        /// </summary>
        [SerializeField] 
        private HidableUiElement ConfigScreenHide;
        
        /// <summary>
        /// Config button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton ConfigButtonSus;

        /// <summary>
        /// Config ok button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton ConfigOkButtonSus;
        
        /// <summary>
        /// Scan for food button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton ScanButtonSus;
        
        /// <summary>
        /// Reference to the scan for images scene to load.
        /// </summary>
        [SerializeField]
        private SceneReference ScanScene;

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

            ScanButtonSus += LoadCameraScene;

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

        private void LoadCameraScene()
        {
            CoroutineRunner.RunRoutine(LoadCameraSceneCoroutine());
        }

        private IEnumerator LoadCameraSceneCoroutine()
        {
            yield return LoadingScreen.FadeIn();
            
            string currentScene = sceneManager.GetActiveSceneName();

            LoadingScreen.SetTitleText(localizer["Common/Title"]);
            LoadingScreen.SetDebugText(localizer["Debug/LoadingCamera"]);
            
            bool loaded = false;

            sceneManager.LoadScene(ScanScene,
                _ =>
                {
                },
                success =>
                {
                    if (!success)
                        Logger.Error("There was an error loading the next scene.");
                    else
                        loaded = true;
                });

            yield return new WaitUntil(() => loaded);

            loaded = false;
            
            sceneManager.UnloadScene(currentScene,
                _ =>
                {
                },
                success =>
                {
                    if (!success)
                        Logger.Error("There was an error unloading the main menu scene.");
                    else
                        loaded = true;
                });

            //yield return new WaitUntil(() => loaded);

            yield return LoadingScreen.FadeOut();
        }
    }
}

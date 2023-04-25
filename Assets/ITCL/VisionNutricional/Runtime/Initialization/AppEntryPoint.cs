using System.Collections;
using ITCL.VisionNutricional.Runtime.UI;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using Zenject;

namespace ITCL.VisionNutricional.Runtime.Initialization
{
    /// <summary>
    /// Class that initializes the game.
    /// </summary>
    public class AppEntryPoint : WhateverBehaviour<AppEntryPoint>
    {
        /// <summary>
        /// Reference to the scene manager.
        /// </summary>
        [Inject] private ISceneManager sceneManager;

        /// <summary>
        /// Reference to the next scene to load.
        /// </summary>
        [SerializeField] private SceneReference NextScene;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject] private ILocalizer localizer;

        /// <summary>
        /// Call game initialization.
        /// </summary>
        private void OnEnable() =>
            CoroutineRunner.RunRoutine(Loader.LoadSceneCoroutine(
                sceneManager, NextScene, localizer["Common/Title"], localizer["Debug/Loading"], 2));
        //CoroutineRunner.RunRoutine(Initialize());

        private IEnumerator Initialize()
        {
            yield return LoadingScreen.FadeIn();

            string currentScene = sceneManager.GetActiveSceneName();

            LoadingScreen.SetTitleText(localizer["Common/Title"]);
            LoadingScreen.SetDebugText(localizer["Debug/Loading"]);

            bool loaded = false;

            sceneManager.LoadScene(NextScene,
                _ => { },
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
                _ => { },
                success =>
                {
                    if (!success)
                        Logger.Error("There was an error unloading the init scene.");
                    else
                        loaded = true;
                });

            yield return new WaitUntil(() => loaded);

            yield return new WaitForSeconds(2);

            yield return LoadingScreen.FadeOut();
        }
    }
}
using System.Collections;
using ITCL.VisionNutricional.Runtime.UI;
using ModestTree;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;

namespace ITCL.VisionNutricional.Runtime.Initialization
{
    /// <summary>
    /// Static method for loading scenes with the loading screen.
    /// </summary>
    public class Loader :Loggable<Loader>
    {
        /// <summary>
        /// Coroutine that manages the loading and unloading of the scenes and the loading screen.
        /// </summary>
        /// <param name="sceneManager">Reference to the scene manager.</param>
        /// <param name="scene">Scene to load next.</param>
        /// <param name="titleText">Title to appear on the loading screen.</param>
        /// <param name="debugText">Subtitle or extra info to appear on the loading screen.</param>
        /// <param name="wait">Seconds of waiting before the unloading of the loading screen after the next scene is loaded.</param>
        /// <returns></returns>
        public static IEnumerator LoadSceneCoroutine(ISceneManager sceneManager, SceneReference scene, string titleText, string debugText, float wait = 0)
        {
            StaticLogger.Debug("Started loading scene");
            yield return LoadingScreen.FadeIn();
            
            string currentScene = sceneManager.GetActiveSceneName();

            LoadingScreen.SetTitleText(titleText);
            LoadingScreen.SetDebugText(debugText);
            
            bool loaded = false;

            sceneManager.LoadScene(scene,
                _ =>
                {
                },
                success =>
                {
                    if (!success)
                        Log.Error("There was an error loading the next scene.");
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
                        Log.Error("There was an error unloading the previous scene.");
                    else
                        loaded = true;
                });

            yield return new WaitUntil(() => loaded);
            
            yield return new WaitForSeconds(wait);

            yield return LoadingScreen.FadeOut();
        }
    }
}

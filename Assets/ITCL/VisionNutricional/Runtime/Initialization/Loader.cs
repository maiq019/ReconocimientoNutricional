using System.Collections;
using ITCL.VisionNutricional.Runtime.UI;
using ModestTree;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using Zenject;

namespace ITCL.VisionNutricional.Runtime.Initialization
{
    public class Loader : WhateverBehaviour<Loader>
    {
        public static IEnumerator LoadSceneCoroutine(ISceneManager sceneManager, SceneReference scene, string titleText, string debugText, float wait = 0)
        {
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

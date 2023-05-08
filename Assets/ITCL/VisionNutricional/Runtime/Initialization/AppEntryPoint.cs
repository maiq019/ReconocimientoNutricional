using System;
using System.Collections;
using System.Globalization;
using ITCL.VisionNutricional.Runtime.DataBase;
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
        /// Loads the selected first scene, the login.
        /// </summary>
        private void OnEnable()
        {
            DB.CreateDataBase();
            CoroutineRunner.RunRoutine(FillDbCoroutine());
            
            CoroutineRunner.RunRoutine(Loader.LoadSceneCoroutine(
                sceneManager, NextScene, localizer["Common/Title"], localizer["Debug/Loading"], 2));
        }

        private IEnumerator FillDbCoroutine()
        {
            DB.CreateDataBase();
            yield return new WaitForEndOfFrame();
            DB.CreateDatabaseTables();
            yield return new WaitForEndOfFrame();
            DB.InsertUser("user0@gmail.com", "user0", "0000");
            DB.InsertUser("user1@gmail.com", "user1", "1111");
            
            
            DB.InsertIntoHistoric("user0@gmail.com", "bread", DateTime.Now.ToString(CultureInfo.InvariantCulture));
            yield return new WaitForEndOfFrame();
        }
    }
}
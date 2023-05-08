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

        private void Awake()
        {
            DB.CreateDataBase();
            CoroutineRunner.RunRoutine(FillDbCoroutine());
        }

        /// <summary>
        /// Loads the selected first scene, the login.
        /// </summary>
        private void OnEnable() =>
            CoroutineRunner.RunRoutine(Loader.LoadSceneCoroutine(
                sceneManager, NextScene, localizer["Common/Title"], localizer["Debug/Loading"], 2));

        private IEnumerator FillDbCoroutine()
        {
            DB.DeleteDatabase();
            DB.CreateDatabaseTables();
            yield return new WaitForEndOfFrame();
            DB.InsertUser("user0@gmail.com", "user0", "0000");
            DB.InsertUser("user1@gmail.com", "user1", "1111");
            DB.InsertFood("bread", 265, 4, 0, 46, 3, 7, 0);
            DB.InsertFood("pasta", 221, 1.3f, -1, 43.2f, 1.7f, 8.1f, 0.05f);
            DB.InsertFood("rice", 121, 0.38f, 0.09f, 25, 0.05f, 3.5f, 0.012f);
            DB.InsertFood("beef meat", 201, 8.1f, 0, 0, 0, 33.8f, 0);
            DB.InsertFood("chicken meat", 158, 10, 0, 0, 0, 23, 0);
            DB.InsertIntoHistoric("user0@gmail.com", "bread", DateTime.Now.ToString(CultureInfo.InvariantCulture));
            yield return new WaitForEndOfFrame();
        }
    }
}
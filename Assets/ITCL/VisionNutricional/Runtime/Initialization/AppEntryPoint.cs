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
            DB.CreateDatabaseTables();
            DB.InsertFood("pasta", 131);
            
        }

        /// <summary>
        /// Loads the selected first scene, the login.
        /// </summary>
        private void OnEnable() =>
            CoroutineRunner.RunRoutine(Loader.LoadSceneCoroutine(
                sceneManager, NextScene, localizer["Common/Title"], localizer["Debug/Loading"], 2));
    }
}
using System.Collections;
using ITCL.VisionNutricional.Runtime.DataBase;
using UnityEngine;
using UnityEngine.Android;
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
        /// Reference to the localizer.
        /// </summary>
        [Inject] private ILocalizer localizer;

        /// <summary>
        /// Reference to the next scene to load.
        /// </summary>
        [SerializeField] private SceneReference NextScene;

        /// <summary>
        /// Reference to the foods asset.
        /// </summary>
        [SerializeField] private ScriptableFood FoodsAsset;
        

        /// <summary>
        /// Loads the selected first scene, the login.
        /// </summary>
        private void Start() => CoroutineRunner.RunRoutine(InitLoad());
        
        private IEnumerator InitLoad()
        {
            yield return Application.platform == RuntimePlatform.Android ? PermissionsAndDBCoroutine() : FillDbCoroutine();
            
            yield return Loader.LoadSceneCoroutine(sceneManager, NextScene, localizer["Common/Title"], localizer["Debug/Loading"], 1);
        }

        private IEnumerator PermissionsAndDBCoroutine()
        {
            while (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageWrite);
                yield return new WaitForSeconds(1);
            }
            while (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageRead);
                yield return new WaitForSeconds(1);
            }
            while (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
                yield return new WaitForSeconds(1);
            }
            
            yield return FillDbCoroutine();
        }

        private IEnumerator FillDbCoroutine()
        {
            yield return DB.CreateDataBase();
            yield return new WaitForEndOfFrame();
            DB.InsertUser("admin@gmail.com", "Admin", "Aa000");
            DB.InsertUser("user1@gmail.com", "User1", "Uu111");

            foreach (DB.Food food in FoodsAsset.Foods) DB.InsertFood(food);
            
            yield return new WaitForEndOfFrame();
        }
    }
}
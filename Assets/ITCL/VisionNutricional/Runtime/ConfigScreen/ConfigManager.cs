using ITCL.VisionNutricional.Runtime.DataBase;
using ITCL.VisionNutricional.Runtime.Initialization;
using ITCL.VisionNutricional.Runtime.Login;
using TMPro;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.Localization.Runtime.Ui;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using Zenject;

namespace ITCL.VisionNutricional.Runtime.ConfigScreen
{
    public class ConfigManager : WhateverBehaviour<ConfigManager>
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
        /// Reference to the Login screen.
        /// </summary>
        [SerializeField] private SceneReference LoginScene;

        /// <summary>
        /// Config back button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton ConfigBackButtonSus;
        
        /// <summary>
        /// Edit user button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton EditUserButtonSus;

        /// <summary>
        /// Edit user screen hidable.
        /// </summary>
        [SerializeField] private HidableUiElement EditUserScreenHid;
        
        /// <summary>
        /// Logout button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton LogoutButtonSus;
        
        #region EditUser

        /// <summary>
        /// Reference to the user email text.
        /// </summary>
        [SerializeField] private LocalizedTextMeshPro UserEmail;

        /// <summary>
        /// Reference to the userName placeholder text.
        /// </summary>
        [SerializeField] private TMP_Text UserNamePlaceholder;

        /// <summary>
        /// reference to the userName input.
        /// </summary>
        [SerializeField] private TMP_InputField UserNameInput;
        
        /// <summary>
        /// reference to the password input.
        /// </summary>
        [SerializeField] private TMP_InputField PasswordInput;
        
        /// <summary>
        /// reference to the repeat password input.
        /// </summary>
        [SerializeField] private TMP_InputField RepeatPasswordInput;

        /// <summary>
        /// Reference to the password error hidable.
        /// </summary>
        [SerializeField] private HidableUiElement PasswdErrorHid;
        
        /// <summary>
        /// Apply user changes button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton ApplyUserButtonSus;
        
        #endregion

        /// <summary>
        /// Subscribes to the OnButtonClicked of the corresponding button.
        /// </summary>
        private void OnEnable()
        {
            EditUserScreenHid.Show(false);
            UserEmail.SetValue("Common/Config/UserEmail", false, Session.Email);
            UserNamePlaceholder.text = Session.UserName;
            PasswdErrorHid.Show(false);
            
            ConfigBackButtonSus += Back;
            EditUserButtonSus += () => EditUserScreenHid.Show();
            LogoutButtonSus += Logout;

            ApplyUserButtonSus += UpdateUser;
        }
        
        /// <summary>
        /// Logs out from the user account and returns to the login screen.
        /// </summary>
        private void Logout()
        {
            LogoutButtonSus -= Logout;
            CoroutineRunner.RunRoutine(
                Loader.LoadSceneCoroutine(sceneManager, LoginScene, localizer["Common/title"], localizer["Debug/Logout"]));
        }

        /// <summary>
        /// Loads the scene from witch config was loaded.
        /// </summary>
        private void LoadPreviousScene()
        { 
            ConfigBackButtonSus -= LoadPreviousScene;
            CoroutineRunner.RunRoutine(
                Loader.LoadSceneCoroutine(sceneManager, Session.PreviousScene, localizer["Common/title"], ""));
        }

        /// <summary>
        /// Checks the password inputs and updates the user in the database if able.
        /// </summary>
        private void UpdateUser()
        {
            if (PasswordInput.text.Equals(RepeatPasswordInput.text))
            {
                DB.UpdateOneUser(Session.Email, Session.Passwd, UserNameInput.text, PasswordInput.text);
                EditUserScreenHid.Show(false);
            }
            else
            {
                PasswdErrorHid.Show();
            }
        }

        private void Back()
        {
            if (EditUserScreenHid.Shown) EditUserScreenHid.Show(false);
            else LoadPreviousScene();
        }
    }
}

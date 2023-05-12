using ITCL.VisionNutricional.Runtime.DataBase;
using ITCL.VisionNutricional.Runtime.Initialization;
using ITCL.VisionNutricional.Runtime.Login;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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
        /// Reference to the username localize.
        /// </summary>
        [SerializeField] private LocalizedTextMeshPro UserName;
        
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
        /// Reference to the update user info message hidable.
        /// </summary>
        [SerializeField] private HidableUiElement UpdateUserInfoHid;

        /// <summary>
        /// Reference to the update user info message localizer.
        /// </summary>
        [SerializeField] private LocalizedTextMeshPro UpdateUserInfoLocalizer;

        /// <summary>
        /// Reference to the update user error message hidable.
        /// </summary>
        [SerializeField] private HidableUiElement UpdateUserErrorHid;

        /// <summary>
        /// Reference to the update user error message localizer.
        /// </summary>
        [SerializeField] private LocalizedTextMeshPro UpdateUserErrorLocalizer;
        
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
            HideUpdateUser();
            UserEmail.SetValue("Common/Config/UserEmail", false, Session.Email);
            UpdateUserErrorHid.Show(false);
            ApplyUserButtonSus += UpdateUser;
            
            ConfigBackButtonSus += Back; //() => EditUserScreenHid.Shown ? HideUpdateUser() : LoadPreviousScene();
            UserName.SetValue("Common/Config/UserName0", false, Session.UserName);
            EditUserButtonSus += ShowUpdateUser;
            LogoutButtonSus += Logout;

            
        }
        
        /// <summary>
        /// Logs out from the user account and returns to the login screen.
        /// </summary>
        private void Logout()
        {
            LogoutButtonSus -= Logout;
            Session.Email = null;
            Session.UserName = null;
            Session.Passwd = null;
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
        /// Shows the update user screen.
        /// </summary>
        private void ShowUpdateUser()
        {
            UserNamePlaceholder.text = Session.UserName;
            UserNameInput.text = "";
            PasswordInput.text = "";
            RepeatPasswordInput.text = "";
            UpdateUserErrorHid.Show(false);
            UpdateUserInfoLocalizer.SetValue("Common/Config/PasswdErrorCharacters");
            UpdateUserInfoHid.Show();
            EditUserScreenHid.Show();
        }

        /// <summary>
        /// Hides the update user screen.
        /// </summary>
        private void HideUpdateUser() => EditUserScreenHid.Show(false);
        
        /// <summary>
        /// Checks the password inputs and updates the user in the database if able.
        /// </summary>
        private void UpdateUser()
        {
            string newUserName = Session.UserName;
            string newPassword = Session.Passwd;

            if (!string.IsNullOrEmpty(UserNameInput.text)) newUserName = UserNameInput.text;

            if (!string.IsNullOrEmpty(PasswordInput.text))
            {
                if (CheckPassword()) newPassword = PasswordInput.text;
                else return;
            }

            if (newUserName.Equals(Session.UserName) && newPassword.Equals(Session.Passwd)) HideUpdateUser();
            else
            {
                switch (newUserName.Equals(Session.UserName))
                {
                    case false when !newPassword.Equals(Session.Passwd):
                        UpdateUserInfoLocalizer.SetValue("Common/Config/UserPasswdUpdated");
                        break;
                    case false:
                        UpdateUserInfoLocalizer.SetValue("Common/Config/UserUpdated");
                        break;
                    default:
                        UpdateUserInfoLocalizer.SetValue("Common/Config/PasswdUpdated");
                        break;
                }
                
                UpdateUserErrorHid.Show(false);
                UpdateUserInfoHid.Show();
                    
                DB.UpdateOneUser(Session.Email, Session.Passwd, newUserName, newPassword);
                Session.UserName = newUserName;
                Session.Passwd = newPassword;
                UserName.SetValue("Common/Config/UserName0", false, Session.UserName);
                UserNamePlaceholder.text = Session.UserName;
                UserNameInput.text = "";
                PasswordInput.text = "";
                RepeatPasswordInput.text = "";
            }
        }

        private bool CheckPassword()
        {
            string newPass = PasswordInput.text;
            if (!newPass.Equals(RepeatPasswordInput.text))
            {
                UpdateUserErrorLocalizer.SetValue("Common/Config/PasswdErrorNotEqual");
                UpdateUserInfoHid.Show(false);
                UpdateUserErrorHid.Show();
                return false;
            }

            if (string.IsNullOrEmpty(newPass)) return true;

            if (newPass.Length < 5)
            {
                UpdateUserErrorLocalizer.SetValue("Common/Config/PasswdErrorLength");
                UpdateUserInfoHid.Show(false);
                UpdateUserErrorHid.Show();
                return false;
            }

            bool hasUpperCaseLetter = false;
            bool hasLowerCaseLetter = false;
            bool hasSpecialCharacter = false;
            foreach (char c in newPass)
            {
                if (char.IsUpper(c)) hasUpperCaseLetter = true;
                if (char.IsLower(c)) hasLowerCaseLetter = true;
                if (char.IsNumber(c) || char.IsPunctuation(c) || char.IsSymbol(c)) hasSpecialCharacter = true;
            }

            if (hasUpperCaseLetter && hasLowerCaseLetter && hasSpecialCharacter) return true;
                
            UpdateUserErrorLocalizer.SetValue("Common/Config/PasswdErrorCharacters");
            UpdateUserInfoHid.Show(false);
            UpdateUserErrorHid.Show();
            return false;
        }

        /// <summary>
        /// Closes the popup screen or loads the previous scene.
        /// </summary>
        private void Back()
        {
            if (EditUserScreenHid.Shown) HideUpdateUser();
            else LoadPreviousScene();
        }
    }
}

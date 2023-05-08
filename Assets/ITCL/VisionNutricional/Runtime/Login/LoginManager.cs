using System.Collections.Generic;
using System.Linq;
using ITCL.VisionNutricional.Runtime.DataBase;
using ITCL.VisionNutricional.Runtime.Initialization;
using ModestTree;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using Zenject;

namespace ITCL.VisionNutricional.Runtime.Login
{
    /// <summary>
    /// Manager for the login and its screen.
    /// </summary>
    public class LoginManager : WhateverBehaviour<LoginManager>
    {
        /// <summary>
        /// Reference to the scene manager.
        /// </summary>
        [Inject] private ISceneManager sceneManager;

        /// <summary>
        /// Reference to the next scene to load.
        /// </summary>
        [SerializeField] private SceneReference MainMenuScene;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject] private ILocalizer localizer;

        /// <summary>
        /// Email from the user account.
        /// </summary>
        private string Email;

        /// <summary>
        /// Reference to the input text box for the email.
        /// </summary>
        [SerializeField] private TMP_InputField EmailInput;

        /// <summary>
        /// Reference to the hidable for the email error.
        /// </summary>
        [SerializeField] private HidableUiElement EmailErrorHid;
        
        /// <summary>
        /// Password from the user account
        /// </summary>
        private string Passwd;
        
        /// <summary>
        /// Reference to the input text box for the password.
        /// </summary>
        [SerializeField] private TMP_InputField PasswdInput;

        /// <summary>
        /// Reference to the hidable for the password error.
        /// </summary>
        [SerializeField] private HidableUiElement PasswdErrorHid;
        
        /// <summary>
        /// Reference to the hidable for the connection error.
        /// </summary>
        [SerializeField] private HidableUiElement ConnectErrorHid;

        /// <summary>
        /// Subscribable to the accept button.
        /// </summary>
        [SerializeField] private EasySubscribableButton EnterSus;

        private void Awake()
        {
            EmailErrorHid.Show(false);
            PasswdErrorHid.Show(false);
            ConnectErrorHid.Show(false);
        }

        private void OnEnable()
        {
            //EnterSus += LoadMainMenu;
            EnterSus += Login;
        }

        /// <summary>
        /// Checks and asks for storage permission.
        /// </summary>
        private void Start()
        {
            if (Application.platform != RuntimePlatform.Android) return;
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            }
        }

        /// <summary>
        /// Exits the app with the android back button.
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        }

        /// <summary>
        /// Reads the inputs for the user account and starts the login.
        /// </summary>
        private void Login()
        {
            EmailErrorHid.Show(false);
            PasswdErrorHid.Show(false);
            
            Email = EmailInput.text.Replace("\u200B", "");
            Passwd = PasswdInput.text.Replace("\u200B", "");
            
            if (string.IsNullOrEmpty(Email)) EmailErrorHid.Show();
            else if (string.IsNullOrEmpty(Passwd)) PasswdErrorHid.Show();
            else ConnectToLogIn();
        }

        /// <summary>
        /// Connects to the database to check the user account and log in to it.
        /// </summary>
        private void ConnectToLogIn()
        {
            bool incorrectEmail = true;

            List<DB.User> users = DB.SelectAllUsers();
            
            foreach (DB.User user in users.Where(user => Email.Equals(user.email)))
            {
                incorrectEmail = false;
                if (!Passwd.Equals(user.password)) continue;
                Session.Email = user.email;
                Session.UserName = user.userName;
                Session.Passwd = user.password;
                LoadMainMenu();
                return;
            }

            if (incorrectEmail) EmailErrorHid.Show();
            else PasswdErrorHid.Show();
        }

        /// <summary>
        /// Loads the scene selected as main menu.
        /// </summary>
        private void LoadMainMenu()
        {
            EnterSus -= LoadMainMenu;
            CoroutineRunner.RunRoutine(Loader.LoadSceneCoroutine(
                sceneManager, MainMenuScene, localizer["Common/Title"], localizer["Debug/LoadingMainMenu"]));
        }
    }
}

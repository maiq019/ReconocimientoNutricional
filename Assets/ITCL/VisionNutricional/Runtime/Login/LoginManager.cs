using System.Collections;
using ITCL.VisionNutricional.Runtime.Initialization;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;
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

        private string Email;

        [SerializeField] private TMP_Text EmailInput;

        [SerializeField] private HidableUiElement EmailErrorHid;
        
        private string Passwd;
        
        [SerializeField] private TMP_Text PasswdInput;

        [SerializeField] private HidableUiElement PasswdErrorHid;
        
        [SerializeField] private HidableUiElement ConnectErrorHid;

        /// <summary>
        /// Subscribable to the accept button
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
            EnterSus += LoadMainMenu;
        }

        private void Start()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
                {
                    Permission.RequestUserPermission(Permission.ExternalStorageWrite);
                }
            }
        }

        /// <summary>
        /// Exits the app with the android back button.
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        }

        private void Login()
        {
            Email = EmailInput.text;
            Passwd = PasswdInput.text;
            
            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Passwd)) StartCoroutine(ConnectToLogInCoroutine());
            else
            {
                if (string.IsNullOrEmpty(Email)) EmailErrorHid.Show();
                if (string.IsNullOrEmpty(Passwd)) PasswdErrorHid.Show();
            }
        }

        private IEnumerator ConnectToLogInCoroutine()
        {
            WWWForm form = new WWWForm();

            string trimEmail = Email.Trim();
            string trimPasswd = Passwd.Trim();
            form.AddField("email", trimEmail);
            form.AddField("password", trimPasswd);
            
            using (UnityWebRequest www =
            UnityWebRequest.Post("http://syltec.dyndns-remote.com:6022/get-session-permission/", form))
            {
                yield return www.SendWebRequest();
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Logger.Debug("CONNECTION ERROR " + www.error);
                    ConnectErrorHid.Show();
                }
                else
                {
                    Logger.Debug("Form upload complete!" + www.downloadHandler.text);
                    Session.CurrentSession = JsonUtility.FromJson<Session.User>(www.downloadHandler.text);
                    Logger.Debug("SessionCode" + Session.CurrentSession.SessionCode);
                    Logger.Debug("Check" + Session.CurrentSession.Check);

                    switch (Session.CurrentSession.Check)
                    {
                        case 1:
                            Session.Email = trimEmail;
                            Session.Passwd = trimPasswd;

                            PlayerPrefs.SetString("Email", trimEmail);
                            PlayerPrefs.SetString("Password", trimPasswd);
                        
                            //PlayerPrefs.SetString("NextScene", "TIA_Menu");
                            //SceneManager.LoadSceneAsync("TIA_Loading");
                            break;
                        case 0:
                            EmailErrorHid.Show();
                            break;
                        case 3:
                            PasswdErrorHid.Show();
                            break;
                    }
                }
            }
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

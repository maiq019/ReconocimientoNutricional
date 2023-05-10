using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ITCL.VisionNutricional.Runtime.DataBase;
using ITCL.VisionNutricional.Runtime.Initialization;
using ITCL.VisionNutricional.Runtime.Login;
using ITCL.VisionNutricional.Runtime.MainMenu;
using TMPro;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.Localization.Runtime.Ui;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;
using Zenject;

namespace ITCL.VisionNutricional.Runtime.Historic
{
    /// <summary>
    /// Manager for Historic and its screens.
    /// </summary>
    public class HistoricManager : WhateverBehaviour<HistoricManager>
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
        /// Factory for the entry buttons.
        /// </summary>
        [Inject] private HistoricEntryManager.Factory buttonFactory;

        /// <summary>
        /// Main button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton MainButtonSus;
        
        /// <summary>
        /// Reference to the next scene to load.
        /// </summary>
        [SerializeField] private SceneReference MainMenuScene;

        /// <summary>
        /// Reference to the search bar input field.
        /// </summary>
        [SerializeField] private TMP_InputField SearchInput;
        
        /// <summary>
        /// Reference to the search error popup hidable.
        /// </summary>
        [SerializeField] private HidableUiElement SearchErrorHid;
        
        /// <summary>
        /// Content field where to place the exercise buttons.
        /// </summary>
        [SerializeField] private Transform Content;

        /// <summary>
        /// Reference to the config popup screen.
        /// </summary>
        [SerializeField] private ConfigManager ConfigWindow;

        /// <summary>
        /// Reference to the entry popup hidable.
        /// </summary>
        [SerializeField] private HidableUiElement EntryPopupHid;

        /// <summary>
        /// Reference to the food name localizer.
        /// </summary>
        [SerializeField] private LocalizedTextMeshPro FoodName;

        /// <summary>
        /// Reference to the calories value.
        /// </summary>
        [SerializeField] private TMP_Text CaloriesValue;

        /// <summary>
        /// Reference to the fat value.
        /// </summary>
        [SerializeField] private TMP_Text FatValue;

        /// <summary>
        /// Reference to the saturated fat value.
        /// </summary>
        [SerializeField] private TMP_Text SatFatValue;

        /// <summary>
        /// Reference to the carbohydrates value.
        /// </summary>
        [SerializeField] private TMP_Text CarbhydValue;

        /// <summary>
        /// Reference to the sugar value.
        /// </summary>
        [SerializeField] private TMP_Text SugarValue;

        /// <summary>
        /// Reference to the protein value.
        /// </summary>
        [SerializeField] private TMP_Text ProteinValue;

        /// <summary>
        /// Reference to the salt value.
        /// </summary>
        [SerializeField] private TMP_Text SaltValue;

        /// <summary>
        /// Reference to the date.
        /// </summary>
        [SerializeField] private TMP_Text Date;

        /// <summary>
        /// Reference to the entry popup's close button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton CloseEntryPopupSus;

        /// <summary>
        /// Subscribes to the OnButtonClicked of the corresponding button.
        /// </summary>
        private void OnEnable()
        {
            ConfigWindow.HideConfigScreen();
            MainButtonSus += LoadMainMenu;

            StartCoroutine(LoadEntriesCoroutine());
            CloseEntryPopupSus += () => EntryPopupHid.Show(false);
        }
        
        /// <summary>
        /// Loads the main menu scene.
        /// </summary>
        private void LoadMainMenu()
        {
            MainButtonSus -= LoadMainMenu;
            CoroutineRunner.RunRoutine(Loader.LoadSceneCoroutine(
                sceneManager, MainMenuScene, localizer["Common/Title"], localizer["Debug/LoadingMainMenu"]));
        }

        /// <summary>
        /// Coroutine witch creates a button for each historic entry.
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadEntriesCoroutine()
        {
            List<DB.HistoricEntry> entries = SearchEntries();
            entries.Sort((e1, e2) => string.Compare(e1.date, e2.date, StringComparison.Ordinal));
            
            foreach (DB.HistoricEntry entry in entries)
            {
                HistoricEntryManager button = buttonFactory.CreateUiGameObject(Content);
                yield return new WaitForEndOfFrame();
                button.SetData(entry);

                button.ButtonSus.OnButtonClicked += () => ShowEntryPopup(entry);
            }
        }
        
        /// <summary>
        /// Gets the needed entries from the database.
        /// </summary>
        /// <returns></returns>
        private List<DB.HistoricEntry> SearchEntries()
        {
            SearchErrorHid.Show(false);
            
            if (!string.IsNullOrEmpty(SearchInput.text)) return DB.SelectAllEntriesFromUser(Session.Email);
            
            List<DB.Food> foodsInDb = DB.SelectAllFoods();
            if (foodsInDb.Any(food => food.foodName.Equals(SearchInput.text)))
                return DB.SelectAllEntriesFromUserAndFood(Session.Email, SearchInput.text);
            
            SearchErrorHid.Show();
            
            return DB.SelectAllEntriesFromUser(Session.Email);
        }

        /// <summary>
        /// Shows a popup with more info from the entry.
        /// </summary>
        /// <param name="entry"></param>
        private void ShowEntryPopup(DB.HistoricEntry entry)
        {
            DB.Food food = DB.SelectFoodByName(entry.foodName);

            FoodName.SetValue("Foods/" + food.foodName);
            CaloriesValue.text = food.calories.ToString();
            FatValue.text = food.fat.ToString();
            SatFatValue.text = food.saturatedFat.ToString();
            CarbhydValue.text = food.carbHyd.ToString();
            SugarValue.text = food.sugar.ToString();
            ProteinValue.text = food.protein.ToString();
            SaltValue.text = food.salt.ToString();
            Date.text = entry.date;
            
            EntryPopupHid.Show();
        }
    }
}

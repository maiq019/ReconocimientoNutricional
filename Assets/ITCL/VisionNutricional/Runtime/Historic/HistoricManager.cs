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
        /// Reference to the search button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton SearchButtonSus;

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
        /// Reference to the historic entry opened.
        /// </summary>
        private DB.HistoricEntry EntryDisplayed;

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
        /// Reference to the entry popup's delete button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton DeleteEntrySus;

        /// <summary>
        /// Reference to the entry popup's close button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton CloseEntryPopupSus;

        /// <summary>
        /// Subscribes to the OnButtonClicked of the corresponding button.
        /// </summary>
        private void OnEnable()
        {
            SearchErrorHid.Show(false);
            ConfigWindow.HideConfigScreen();
            EntryPopupHid.Show(false);
            StartCoroutine(LoadEntriesCoroutine());

            MainButtonSus += LoadMainMenu;
            SearchButtonSus += () => StartCoroutine(LoadEntriesCoroutine());
            DeleteEntrySus += DeleteEntry;
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
            List<DB.HistoricEntry> entries = SearchFood();
            yield return new WaitForEndOfFrame();
            if (entries == null) yield break;

            ClearContent();

            entries.Sort((e1, e2) => string.Compare(e2.date, e1.date, StringComparison.Ordinal));

            foreach (DB.HistoricEntry entry in entries)
            {
                HistoricEntryManager button = buttonFactory.CreateUiGameObject(Content);
                yield return new WaitForEndOfFrame();
                button.SetData(entry);

                button.ButtonSus.OnButtonClicked += () => ShowEntryPopup(entry);
            }
        }

        /// <summary>
        /// Deletes all the entry buttons.
        /// </summary>
        private void ClearContent()
        {
            foreach (Transform child in Content) Destroy(child.gameObject);
        }

        /// <summary>
        /// Gets the needed entries from the database.
        /// </summary>
        /// <returns></returns>
        private List<DB.HistoricEntry> SearchFood()
        {
            SearchErrorHid.Show(false);

            //If searcher is empty get all entries
            string input = SearchInput.text.Replace("\u200B", "");
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input)) return DB.SelectAllEntriesFromUser(Session.Email);

            //Search for coincidences in database
            List<DB.Food> foodsInDb = DB.SelectAllFoods();
            foreach (DB.Food food in foodsInDb.Where(food => input.IndexOf(food.foodName, StringComparison.OrdinalIgnoreCase) >= 0))
            {
                return DB.SelectAllEntriesFromUserAndFood(Session.Email, food.foodName);
            }

            //Search for coincidences in all languages
            List<string> foodsInLocalizer = foodsInDb.Select(food => "Foods/" + food.foodName).ToList();
            foreach (string foodInDb in from key in foodsInLocalizer
                     from language in localizer.GetAllLanguageIds()
                     where input.IndexOf(localizer.GetText(key, language), StringComparison.OrdinalIgnoreCase) >= 0
                     select key.Replace("Foods/", ""))
            {
                return DB.SelectAllEntriesFromUserAndFood(Session.Email, foodInDb);
            }

            SearchErrorHid.Show();

            return null;
        }

        private List<string> GetTranslatedFoods()
        {
            List<string> translatedFoods = new();
            List<DB.Food> foodsInDb = DB.SelectAllFoods();
            List<string> foodsInLocalizer = foodsInDb.Select(food => "Foods/" + food.foodName).ToList();

            Dictionary<string, List<string>> dic = localizer.GetTexts(foodsInLocalizer, localizer.GetAllLanguageIds());

            foreach (string language in localizer.GetAllLanguageIds())
            {
                dic[language].ForEach(translatedFoods.Add);
            }

            return translatedFoods;
        }

        /// <summary>
        /// Shows a popup with more info from the entry.
        /// </summary>
        /// <param name="entry"></param>
        private void ShowEntryPopup(DB.HistoricEntry entry)
        {
            EntryDisplayed = entry;

            DB.Food food = DB.SelectFoodByName(entry.foodName);

            FoodName.SetValue("Foods/" + food.foodName);
            CaloriesValue.text = food.calories.ToString() + "Kcal";
            FatValue.text = food.fat.ToString() + "g";
            SatFatValue.text = food.saturatedFat.ToString() + "g";
            CarbhydValue.text = food.carbHyd.ToString() + "g";
            SugarValue.text = food.sugar.ToString() + "g";
            ProteinValue.text = food.protein.ToString() + "g";
            SaltValue.text = food.salt.ToString() + "g";
            Date.text = entry.date;

            EntryPopupHid.Show();
        }

        private void DeleteEntry()
        {
            DB.DeleteEntry(EntryDisplayed);
            EntryPopupHid.Show(false);
            StartCoroutine(LoadEntriesCoroutine());
        }
    }
}
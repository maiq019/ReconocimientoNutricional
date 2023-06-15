using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ITCL.VisionNutricional.Runtime.DataBase;
using ITCL.VisionNutricional.Runtime.Login;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime.Ui;

namespace ITCL.VisionNutricional.Runtime.Historic
{
    public class WindowGraph : WhateverBehaviour<WindowGraph>
    {
        /// <summary>
        /// Reference to the display time selector button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton TimeDisplayButton;

        /// <summary>
        /// Reference to the time selection display hidable.
        /// </summary>
        [SerializeField] private HidableUiElement TimeDisplaySelectionHid;
        
        /// <summary>
        /// Reference to the day display time selector button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton DayDisplayButton;
        
        /// <summary>
        /// Reference to the month display time selector button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton MonthDisplayButton;

        /// <summary>
        /// Reference to the selected display time localized text.
        /// </summary>
        [SerializeField] private LocalizedTextMeshPro DisplaySelectedText;
        
        /// <summary>
        /// Reference to the entries shown localized text.
        /// </summary>
        [SerializeField] private LocalizedTextMeshPro EntriesShownText;
        
        #region GraphBars

        /// <summary>
        /// Reference to the calories bar graph rect transform.
        /// </summary>
        [SerializeField] private RectTransform CaloriesBar;

        /// <summary>
        /// Reference to the calories value inner text.
        /// </summary>
        [SerializeField] private TMP_Text CaloriesValueIn;
        
        /// <summary>
        /// Reference to the calories value outer text.
        /// </summary>
        [SerializeField] private TMP_Text CaloriesValueOut;
        
        /// <summary>
        /// Reference to the fat bar graph rect transform.
        /// </summary>
        [SerializeField] private RectTransform FatBar;
        
        /// <summary>
        /// Reference to the fat value inner text.
        /// </summary>
        [SerializeField] private TMP_Text FatValueIn;
        
        /// <summary>
        /// Reference to the fat value outer text.
        /// </summary>
        [SerializeField] private TMP_Text FatValueOut;
        
        /// <summary>
        /// Reference to the saturated fat bar graph rect transform.
        /// </summary>
        [SerializeField] private RectTransform SatFatBar;
        
        /// <summary>
        /// Reference to the saturated fat value inner text.
        /// </summary>
        [SerializeField] private TMP_Text SatFatValueIn;
        
        /// <summary>
        /// Reference to the saturated fat value outer text.
        /// </summary>
        [SerializeField] private TMP_Text SatFatValueOut;
        
        /// <summary>
        /// Reference to the carbohydrates bar graph rect transform.
        /// </summary>
        [SerializeField] private RectTransform CarbHydBar;
        
        /// <summary>
        /// Reference to the carbohydrates value inner text.
        /// </summary>
        [SerializeField] private TMP_Text CarbHydValueIn;
        
        /// <summary>
        /// Reference to the carbohydrates value outer text.
        /// </summary>
        [SerializeField] private TMP_Text CarbHydValueOut;
        
        /// <summary>
        /// Reference to the sugar bar graph rect transform.
        /// </summary>
        [SerializeField] private RectTransform SugarBar;
        
        /// <summary>
        /// Reference to the sugar value inner text.
        /// </summary>
        [SerializeField] private TMP_Text SugarValueIn;
        
        /// <summary>
        /// Reference to the sugar value outer text.
        /// </summary>
        [SerializeField] private TMP_Text SugarValueOut;
        
        /// <summary>
        /// Reference to the protein bar graph rect transform.
        /// </summary>
        [SerializeField] private RectTransform ProteinBar;
        
        /// <summary>
        /// Reference to the protein value inner text.
        /// </summary>
        [SerializeField] private TMP_Text ProteinValueIn;
        
        /// <summary>
        /// Reference to the protein value outer text.
        /// </summary>
        [SerializeField] private TMP_Text ProteinValueOut;
        
        /// <summary>
        /// Reference to the salt bar graph rect transform.
        /// </summary>
        [SerializeField] private RectTransform SaltBar;
        
        /// <summary>
        /// Reference to the salt value inner text.
        /// </summary>
        [SerializeField] private TMP_Text SaltValueIn;
        
        /// <summary>
        /// Reference to the salt value outer text.
        /// </summary>
        [SerializeField] private TMP_Text SaltValueOut;
        
        #endregion
        
        /// <summary>
        /// Reference to the show graph button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton ShowGraphButtonSus;
        
        /// <summary>
        /// Reference to the hide graph button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton HideGraphButtonSus;

        /// <summary>
        /// Reference to the number of entries shown in the graph.
        /// </summary>
        private int EntryCount;

        /// <summary>
        /// Reference to the graph container width.
        /// </summary>
        private const float GraphContainerWidth = 913f;


        private void OnEnable()
        {
            GetComponent<HidableUiElement>().Show(false);
            DisplaySelectedText.SetValue("Common/Historic/Graph/Month");
            
            ShowGraphButtonSus += () => MakeGraph("month");
            HideGraphButtonSus += () => GetComponent<HidableUiElement>().Show(false);

            TimeDisplayButton += () => TimeDisplaySelectionHid.Show();
            DayDisplayButton += SelectDay;
            MonthDisplayButton += SelectMonth;
        }

        /// <summary>
        /// Display the day graph and sets the indicator accordingly.
        /// </summary>
        private void SelectDay()
        {
            TimeDisplaySelectionHid.Show(false);
            DisplaySelectedText.SetValue("Common/Historic/Graph/Day");
            MakeGraph("day");
        }

        /// <summary>
        /// Display the month graph and sets the indicator accordingly.
        /// </summary>
        private void SelectMonth()
        {
            TimeDisplaySelectionHid.Show(false);
            DisplaySelectedText.SetValue("Common/Historic/Graph/Month");
            MakeGraph("month");
        }

        /// <summary>
        /// Gets the needed entries for the selected time graph and displays it. 
        /// </summary>
        /// <param name="limit">"day" for today's values, "month" for this month values.</param>
        private void MakeGraph(string limit)
        {
            List<float> valuesList = new(){0, 0, 0, 0, 0, 0, 0};
            List<DB.HistoricEntry> entriesList = DB.SelectAllEntriesFromUser(Session.Email);

            EntryCount = 0;
            
            foreach (DB.HistoricEntry entry in entriesList)
            {
                switch (limit)
                {
                    case "month" when !int.TryParse(entry.date.Substring(3, 2), out int month) || !month.Equals(DateTime.Now.Month):
                        continue;
                    case "month":
                        valuesList[0] += entry.calories;
                        valuesList[1] += entry.fat;
                        valuesList[2] += entry.saturatedFat;
                        valuesList[3] += entry.carbHyd;
                        valuesList[4] += entry.sugar;
                        valuesList[5] += entry.protein;
                        valuesList[6] += entry.salt;
                        EntryCount++;
                        break;
                    case "day" when !int.TryParse(entry.date[..2], out int day) || !day.Equals(DateTime.Now.Day):
                        continue;
                    case "day":
                        valuesList[0] += entry.calories;
                        valuesList[1] += entry.fat;
                        valuesList[2] += entry.saturatedFat;
                        valuesList[3] += entry.carbHyd;
                        valuesList[4] += entry.sugar;
                        valuesList[5] += entry.protein;
                        valuesList[6] += entry.salt;
                        EntryCount++;
                        break;
                }
            }
            
            EntriesShownText.SetValue("Common/Historic/Graph/EntriesCount", false, EntryCount.ToString());

            FillGraph(valuesList);
            
            GetComponent<HidableUiElement>().Show();
        }

        /// <summary>
        /// Fits the graph bars according to the values received.
        /// </summary>
        /// <param name="valueList">List of values to represent in the graph.</param>
        [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
        private void FillGraph(IReadOnlyList<float> valueList)
        {
            float calories = Mathf.Round(valueList[0] * 100.0f) * 0.01f;
            CaloriesBar.pivot = new Vector2(0, 0.5f);
            float barLength = calories * GraphContainerWidth / (EntryCount * 300);
            CaloriesBar.sizeDelta = new Vector2(barLength, 150);
            
            if (barLength > 300)
            {
                CaloriesValueIn.text = calories + "Kcal";
                CaloriesValueOut.text = "";
            }
            else 
            {
                CaloriesValueIn.text = "";
                CaloriesValueOut.text = " " + calories + "Kcal";
            }

            SetBar(FatBar, FatValueIn, FatValueOut, valueList[1]);
            
            SetBar(SatFatBar, SatFatValueIn, SatFatValueOut, valueList[2]);
            
            SetBar(CarbHydBar, CarbHydValueIn, CarbHydValueOut, valueList[3]);
            
            SetBar(SugarBar, SugarValueIn, SugarValueOut, valueList[4]);

            SetBar(ProteinBar, ProteinValueIn, ProteinValueOut, valueList[5]);

            SetBar(SaltBar, SaltValueIn, SaltValueOut, valueList[6]);
        }

        private void SetBar(RectTransform bar, TMP_Text barTextIn, TMP_Text barTextOut, float barValue)
        {
            barValue = Mathf.Round(barValue * 100.0f) * 0.01f;
            bar.pivot = new Vector2(0, 0.5f);
            float barLength = barValue * GraphContainerWidth / (EntryCount * 500);
            bar.sizeDelta = new Vector2(barLength, 150);
            
            if (barLength > 300)
            {
                barTextIn.text = barValue + "g";
                barTextOut.text = "";
            }
            else 
            {
                barTextIn.text = "";
                barTextOut.text = " " + barValue + "g";
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ITCL.VisionNutricional.Runtime.DataBase;
using ITCL.VisionNutricional.Runtime.Login;
using ITCL.VisionNutricional.Runtime.UI;
using ModestTree;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.Localization.Runtime.Ui;
using Zenject;

namespace ITCL.VisionNutricional.Runtime.Camera
{
    public class CloudReceiver : WhateverBehaviour<CloudReceiver>
    {
        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject] private ILocalizer localizer;
        
        #region RectangleButton
        /// <summary>
        /// Reference to the rectangle object transform.
        /// </summary>
        [SerializeField] private GameObject Rectangle;

        /// <summary>
        /// Reference to the text identifier of the rectangle.
        /// </summary>
        [SerializeField] private LocalizedTextMeshPro RectangleText;

        /// <summary>
        /// Reference to the hidable for the rectangle.
        /// </summary>
        public HidableUiElement RectangleHid;
        
        /// <summary>
        /// Reference to the rectangle button subscribable.
        /// </summary>
        private EasySubscribableButton RectangleSus;
        
        /// <summary>
        /// Reference to the rectangle object transform.
        /// </summary>
        private RectTransform RectangleRectTransform;
        
        /// <summary>
        /// Factory for the vertical bar.
        /// </summary>
        [Inject] private VerticalBar.Factory VerticalBarFactory;
        
        /// <summary>
        /// Factory for the horizontal bar.
        /// </summary>
        [Inject] private HorizontalBar.Factory HorizontalBarFactory;
        
        /// <summary>
        /// Width of the rectangle bars.
        /// </summary>
        [SerializeField] private float RectangleWidth = 100;

        /// <summary>
        /// Offset of the rectangle bars.
        /// </summary>
        [SerializeField] private float RectangleOffsize = 15;
        
        #endregion
        
        #region EntryPopup
        /// <summary>
        /// Reference to the food name localizer.
        /// </summary>
        [SerializeField] private LocalizedTextMeshPro FoodName;

        /// <summary>
        /// Reference to the food selection button.
        /// </summary>
        [SerializeField] private EasySubscribableButton FoodSelectionButton;

        /// <summary>
        /// Reference to the food name selector hidable.
        /// </summary>
        [SerializeField] private HidableUiElement FoodSelectionHid;
        
        /// <summary>
        /// Factory for the entry buttons.
        /// </summary>
        [Inject] private SelectableFood.Factory SelectableButtonFactory;
        
        /// <summary>
        /// Content field where to place the name buttons.
        /// </summary>
        [SerializeField] private Transform Content;

        /// <summary>
        /// Reference to the calories value.
        /// </summary>
        [SerializeField] private TMP_InputField CaloriesValue;

        /// <summary>
        /// Reference to the fat value.
        /// </summary>
        [SerializeField] private TMP_InputField FatValue;

        /// <summary>
        /// Reference to the saturated fat value.
        /// </summary>
        [SerializeField] private TMP_InputField SatFatValue;

        /// <summary>
        /// Reference to the carbohydrates value.
        /// </summary>
        [SerializeField] private TMP_InputField CarbhydValue;

        /// <summary>
        /// Reference to the sugar value.
        /// </summary>
        [SerializeField] private TMP_InputField SugarValue;

        /// <summary>
        /// Reference to the protein value.
        /// </summary>
        [SerializeField] private TMP_InputField ProteinValue;

        /// <summary>
        /// Reference to the salt value.
        /// </summary>
        [SerializeField] private TMP_InputField SaltValue;
        
        /// <summary>
        /// Reference to the hidable for the entry popup.
        /// </summary>
        [SerializeField] public HidableUiElement EntryPopupHid;
        
        /// <summary>
        /// Reference to the hidable for the entry format error.
        /// </summary>
        [FormerlySerializedAs("FormatErrorHid")] [SerializeField] public HidableUiElement EntryErrorHid;

        /// <summary>
        /// Reference to the entry intro error localizer.
        /// </summary>
        [SerializeField] private LocalizedTextMeshPro EntryErrorLocalizer;
        
        /// <summary>
        /// Reference to the entry popup's cancel button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton CancelEntrySus;

        /// <summary>
        /// Reference to the entry popup's register button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton RegisterEntryPopupSus;

        #endregion

        /// <summary>
        /// Reference to the capture error message hidable.
        /// </summary>
        [SerializeField] public HidableUiElement ErrorMessageHide;
        
        /// <summary>
        /// Reference to the capture error message localizer.
        /// </summary>
        [SerializeField] private LocalizedTextMeshPro ErrorMessageLocalizer;
        
        /// <summary>
        /// Reference to the capture success message hidable.
        /// </summary>
        [SerializeField] public HidableUiElement SuccessMessageHide;
        
        /// <summary>
        /// Reference to the capture success message localizer.
        /// </summary>
        [SerializeField] private LocalizedTextMeshPro SuccessMessageLocalizer;
        
        /// <summary>
        /// Reference to the food found name.
        /// </summary>
        private string FoundFoodName;

        /// <summary>
        /// Flag to know if a food has been found in the database.
        /// </summary>
        private bool FoodFound;


        private void Awake()
        {
            RectangleSus = Rectangle.GetComponent<EasySubscribableButton>();
            RectangleHid = Rectangle.GetComponent<HidableUiElement>();
            RectangleRectTransform = Rectangle.GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            ErrorMessageHide.Show(false);
            SuccessMessageHide.Show(false);
            RectangleHid.Show(false);
            EntryPopupHid.Show(false);
            EntryErrorHid.Show(false);
            FoodSelectionHid.Show(false);
            
            
            CamTextureToCloudVision.OnCloudResponse += CloudResponse;
            RectangleSus += RectangleClicked;
            
            FoodSelectionButton += () => FoodSelectionHid.Show();
            
            CancelEntrySus += ()=> EntryPopupHid.Show(false);
            RegisterEntryPopupSus += RegisterEntry;
        }

        /// <summary>
        /// Responsible for the interpretation of the cloud api response.
        /// </summary>
        /// <param name="responses">Cloud api responses.</param>
        [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
        private void CloudResponse(CamTextureToCloudVision.AnnotateImageResponses responses)
        {
            ErrorMessageHide.Show(false);
            SuccessMessageHide.Show(false);
            List<CamTextureToCloudVision.Vertex> vertexList = new();
            
            //Checks for lack of responses.
            if (responses.responses.Count <= 0) return;
            //Checks the labels from the responses, this is the objects detected on the image.
            if (responses.responses[0].labelAnnotations is { Count: > 0 })
            {
                //gets all the labels
                List<CamTextureToCloudVision.EntityAnnotation> labels = responses.responses[0].labelAnnotations.ToList();
                //Filters the label descriptions with a relevant score, > 70%
                List<string> labelsDescriptions = (from label in labels where label.score > 0.7 select label.description).ToList();

                if (!labelsDescriptions.Contains("Food"))
                {
                    Logger.Debug("Didn't find a food in the image");
                    ErrorMessageHide.Show();
                    ErrorMessageLocalizer.SetValue("Common/Camera/NoFood");
                    return;
                }

                DB.Food foodFound;
                List<string> commons = labelsDescriptions.Intersect(DB.SelectAllFoodNames()).ToList();
                if (commons.Count > 0)
                {
                    Logger.Debug(commons.Count > 1 ? "More than one match in the database" : "Only one match in the database");

                    foodFound = DB.SelectFoodByName(commons[0]);
                    FoodName.SetValue("Common/Camera/EntryPopup/FoodFound", false, foodFound.foodName);
                    CaloriesValue.text = foodFound.calories.ToString() != "-1" ? foodFound.calories.ToString() : "";
                    FatValue.text = foodFound.fat.ToString() != "-1" ? foodFound.fat.ToString() : "";
                    SatFatValue.text = foodFound.saturatedFat.ToString() != "-1" ? foodFound.saturatedFat.ToString() : "";
                    CarbhydValue.text = foodFound.carbHyd.ToString() != "-1" ? foodFound.carbHyd.ToString() : "";
                    SugarValue.text = foodFound.sugar.ToString() != "-1" ? foodFound.sugar.ToString() : "";
                    ProteinValue.text = foodFound.protein.ToString() != "-1" ? foodFound.protein.ToString() : "";
                    SaltValue.text = foodFound.salt.ToString() != "-1" ? foodFound.salt.ToString() : "";
                    
                    //Gets the Food object found location
                    List<CamTextureToCloudVision.EntityAnnotation> localizedObjects = responses.responses[0].localizedObjectAnnotations.ToList();
                    foreach (CamTextureToCloudVision.EntityAnnotation obj in localizedObjects.Where(_ => name.Contains(foodFound.foodName)))
                    {
                        vertexList = obj.boundingPoly.normalizedVertices;
                    }

                    if (vertexList.IsEmpty())
                    {
                        foreach (CamTextureToCloudVision.EntityAnnotation obj in localizedObjects)
                        {
                            if (obj.name.Equals("Food")) vertexList = obj.boundingPoly.normalizedVertices;
                        }
                    }

                    FoodFound = true;
                    DrawObject(vertexList, foodFound.foodName);
                }
                else //Didnt found correspondences in the database
                {
                    Logger.Debug("No matches on the database");
                    ErrorMessageHide.Show();
                    ErrorMessageLocalizer.SetValue("Common/Camera/NoMatch");
                    
                    

                    StartCoroutine(FillSelectableNamesCoroutine(labelsDescriptions));
                    
                    //Gets the first object location
                    List<CamTextureToCloudVision.EntityAnnotation> localizedObjects = responses.responses[0].localizedObjectAnnotations.ToList();
                    vertexList = localizedObjects[0].boundingPoly.normalizedVertices;
                    FoodFound = false;
                    DrawObject(vertexList, localizer["Common/Camera/SelectFood"]);
                }
            }
            else
            {
                Logger.Debug("Didn't find anything in the image");
            }
        }

        /// <summary>
        /// Coroutine that creates and sets buttons to select the food found name.
        /// </summary>
        /// <param name="nameList"></param>
        /// <returns></returns>
        private IEnumerator FillSelectableNamesCoroutine(List<string> nameList)
        {
            foreach (string foodName in nameList)
            {
                SelectableFood selectable = SelectableButtonFactory.CreateUiGameObject(Content);
                yield return new WaitForEndOfFrame();
                selectable.SetFood(foodName);
                        
                selectable.ButtonSus.OnButtonClicked += () => SelectFoodName(foodName);
            }
        }

        /// <summary>
        /// Auxiliary function to set the selected food name.
        /// </summary>
        /// <param name="foodName"></param>
        private void SelectFoodName(string foodName)
        {
            FoundFoodName = foodName;
            FoodName.SetValue(FoundFoodName);
            FoodSelectionHid.Show(false);
        }

        /// <summary>
        /// Places the lines around the list of vertices provided, forming a rectangle. 
        /// </summary>
        /// <param name="vertices">List of vertices in a 2d space.</param>
        /// <param name="foodName">Name of the food drawn.</param>
        private void DrawObject(List<CamTextureToCloudVision.Vertex> vertices, string foodName)
        {
            RectangleHid.Show();

            if (FoodFound) RectangleText.SetValue("Foods/" + foodName);
            else RectangleText.SetValue(foodName);
            
            FoundFoodName = foodName;
                
            float top = 1f;
            float bot = 0f;
            float left = 1f;
            float right = 0f;
            
            foreach (CamTextureToCloudVision.Vertex vert in vertices)
            {
                if (vert.y > bot) bot = vert.y;
                if (vert.y < top) top = vert.y;
                if (vert.x > right) right = vert.x;
                if (vert.x < left) left = vert.x;
            }
            
            //1-verticalAxis because vertical axis is inverted.
            RectangleRectTransform.sizeDelta = new Vector2(0, 0);
            RectangleRectTransform.anchorMin = new Vector2(left, 1-bot);
            RectangleRectTransform.anchorMax = new Vector2(right, 1-top);
            
            VerticalBar leftSide = VerticalBarFactory.CreateUiGameObject(RectangleRectTransform);
            leftSide.Set(RectangleWidth, RectangleOffsize, 0, 0, 1);
            
            VerticalBar rightSide = VerticalBarFactory.CreateUiGameObject(RectangleRectTransform);
            rightSide.Set(RectangleWidth, RectangleOffsize, 1, 0, 1);

            HorizontalBar topSide = HorizontalBarFactory.CreateUiGameObject(RectangleRectTransform);
            topSide.Set(RectangleWidth, RectangleOffsize, 0, 0, 1);
            
            HorizontalBar botSide = HorizontalBarFactory.CreateUiGameObject(RectangleRectTransform);
            botSide.Set(RectangleWidth, RectangleOffsize, 1, 0, 1);
        }

        private void RectangleClicked()
        {
            if (FoodFound) FoodName.SetValue("Foods/"+FoundFoodName);
            else FoodName.SetValue("Common/Camera/SelectFood");
            FoodSelectionHid.Show(false);
            EntryErrorHid.Show(false);
            EntryPopupHid.Show();
        }

        /// <summary>
        /// Saves the food entry into the database.
        /// </summary>
        private void RegisterEntry()
        {
            EntryErrorHid.Show(false);
            try
            {
                float calories = float.Parse(CaloriesValue.text); //string.IsNullOrEmpty(CaloriesValue.text) ? 0 : float.Parse(CaloriesValue.text),
                float fat = float.Parse(FatValue.text); //string.IsNullOrEmpty(FatValue.text) ? 0 : float.Parse(FatValue.text),
                float saturatedFat = float.Parse(SatFatValue.text); //string.IsNullOrEmpty(SatFatValue.text) ? 0 : float.Parse(SatFatValue.text),
                float carbHyd = float.Parse(CarbhydValue.text); //string.IsNullOrEmpty(CarbhydValue.text) ? 0 : float.Parse(CarbhydValue.text),
                float sugar = float.Parse(SugarValue.text); //string.IsNullOrEmpty(SugarValue.text) ? 0 : float.Parse(SugarValue.text),
                float protein = float.Parse(ProteinValue.text); //string.IsNullOrEmpty(ProteinValue.text) ? 0 : float.Parse(ProteinValue.text),
                float salt = float.Parse(SaltValue.text); //string.IsNullOrEmpty(SaltValue.text) ? 0 : float.Parse(SaltValue.text)
                
                DB.Food foodEntry = new()
                {
                    foodName = FoundFoodName,
                    calories = calories,
                    fat = fat,
                    saturatedFat = saturatedFat,
                    carbHyd = carbHyd,
                    sugar = sugar,
                    protein = protein,
                    salt = salt
                };

                if (calories + fat + saturatedFat + carbHyd + sugar + protein + salt > 100)
                {
                    EntryErrorLocalizer.SetValue("Common/Camera/EntryPopup/ValueError");
                    EntryErrorHid.Show();
                }
                else
                {
                    if (!FoodFound) DB.InsertFood(foodEntry);
                    
                    DB.InsertIntoHistoric(Session.Email, foodEntry);
                    
                    EntryPopupHid.Show(false);
                    
                    SuccessMessageLocalizer.SetValue("Common/Camera/EntryDone");
                    SuccessMessageHide.Show();
                    
                }
            }
            catch (FormatException)
            {
                EntryErrorLocalizer.SetValue("Common/Camera/EntryPopup/FormatError");
                EntryErrorHid.Show();
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }
        }
    }
}

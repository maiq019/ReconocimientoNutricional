using System;
using System.Collections.Generic;
using System.Linq;
using ITCL.VisionNutricional.Runtime.DataBase;
using ITCL.VisionNutricional.Runtime.UI;
using ModestTree;
using TMPro;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime.Ui;
using Zenject;

namespace ITCL.VisionNutricional.Runtime.Camera
{
    public class CloudReceiver : WhateverBehaviour<CloudReceiver>
    {
        /// <summary>
        /// Reference to the food name localizer.
        /// </summary>
        [SerializeField] private LocalizedTextMeshPro FoodName;

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
        /// Reference to the entry popup's cancel button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton CancelEntrySus;

        /// <summary>
        /// Reference to the entry popup's register button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton RegisterEntryPopupSus;

        /// <summary>
        /// Reference to the hidable for the rectangle.
        /// </summary>
        [SerializeField] public HidableUiElement RectangleHid;
        
        /// <summary>
        /// Factory for the vertical bar.
        /// </summary>
        [Inject] private VerticalBar.Factory VerticalBarFactory;
        
        /// <summary>
        /// Factory for the horizontal bar.
        /// </summary>
        [Inject] private HorizontalBar.Factory HorizontalBarFactory;

        /// <summary>
        /// Reference to the rectangle object transform.
        /// </summary>
        [SerializeField] private Transform Rectangle;

        [SerializeField] private float RectangleWidth = 100;

        [SerializeField] private float RectangleOffsize = 15;
        

        private void OnEnable()
        {
            RectangleHid.Show(false);
        }

        private void TestCloudResponse(CamTextureToCloudVision.AnnotateImageResponses responses)
        {
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
                    Log.Debug("Didn't find a food in the image");
                    return;
                }

                DB.Food foodFound = default;
                List<string> commons = (List<string>)labelsDescriptions.Intersect(DB.SelectAllFoodNames());
                if (commons.Count > 0)
                {
                    if (commons.Count > 1) //More than one food corresponding in the database
                    {
                        
                    }
                    else
                    {
                        foodFound = DB.SelectFoodByName(commons[0]);
                        FoodName.SetValue("Common/Camera/FoodFound", false, foodFound.foodName);
                        CaloriesValue.text = foodFound.calories.ToString();
                        FatValue.text = foodFound.fat.ToString();
                        SatFatValue.text = foodFound.saturatedFat.ToString();
                        CarbhydValue.text = foodFound.carbHyd.ToString();
                        SugarValue.text = foodFound.sugar.ToString();
                        ProteinValue.text = foodFound.protein.ToString();
                        SaltValue.text = foodFound.salt.ToString();
                    }
                }
                else //Didnt found correspondences in the database
                {
                    
                }
                

                //Gets the Food object found location
                List<CamTextureToCloudVision.Vertex> objVertex = new();
                List<CamTextureToCloudVision.EntityAnnotation> objects = responses.responses[0].localizedObjectAnnotations.ToList();
                foreach (CamTextureToCloudVision.EntityAnnotation obj in objects.Where(obj => name.Equals(foodFound.foodName)))
                {
                    objVertex = obj.boundingPoly.normalizedVertices;
                }

                if (objVertex.IsEmpty())
                {
                    foreach (CamTextureToCloudVision.EntityAnnotation obj in objects.Where(obj => name.Equals("Food")))
                    {
                        objVertex = obj.boundingPoly.normalizedVertices;
                    }
                }
                
                DrawObject(objVertex);
                 
            }
            
        }

        /// <summary>
        /// Places the lines around the list of vertices provided, forming a rectangle. 
        /// </summary>
        /// <param name="vertices">List of vertices in a 2d space.</param>
        public void DrawObject(List<CamTextureToCloudVision.Vertex> vertices)
        {
            RectangleHid.Show();
                
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
            
            //1-verticalAxis because vertical axis is inverted in unity.
            VerticalBar leftSide = VerticalBarFactory.CreateUiGameObject(Rectangle);
            leftSide.Set(RectangleWidth, RectangleOffsize, left, 1-bot, 1-top);
            
            VerticalBar rightSide = VerticalBarFactory.CreateUiGameObject(Rectangle);
            rightSide.Set(RectangleWidth, RectangleOffsize, right, 1-bot, 1-top);

            HorizontalBar topSide = HorizontalBarFactory.CreateUiGameObject(Rectangle);
            topSide.Set(RectangleWidth, RectangleOffsize, 1-top, left, right);
            
            HorizontalBar botSide = HorizontalBarFactory.CreateUiGameObject(Rectangle);
            botSide.Set(RectangleWidth, RectangleOffsize, 1-bot, left, right);
        }
    }
}

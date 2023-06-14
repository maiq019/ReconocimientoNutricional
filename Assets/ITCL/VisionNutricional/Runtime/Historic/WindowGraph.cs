using System;
using System.Collections;
using System.Collections.Generic;
using ITCL.VisionNutricional.Runtime.DataBase;
using ITCL.VisionNutricional.Runtime.Login;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Ui;
using Zenject;

namespace ITCL.VisionNutricional.Runtime.Historic
{
    public class WindowGraph : WhateverBehaviour<WindowGraph>
    {
        /// <summary>
        /// Factory for the entry buttons.
        /// </summary>
        [Inject] private GraphPoint.Factory PointFactory;
        
        /// <summary>
        /// Content field where to place the entry buttons.
        /// </summary>
        [SerializeField] private Transform Content;
        
        /// <summary>
        /// Reference to the show graph button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton ShowGraphButtonSus;
        
        /// <summary>
        /// Reference to the hide graph button subscribable.
        /// </summary>
        [SerializeField] private EasySubscribableButton HideGraphButtonSus;


        private void OnEnable()
        {
            ShowGraphButtonSus += MakeGraph;
            HideGraphButtonSus += () => GetComponent<HidableUiElement>().Show(false);
        }

        private void MakeGraph()
        {
            
            
            List<DB.HistoricEntry> EntriesList = DB.SelectAllEntriesFromUser(Session.Email);
            foreach (DB.HistoricEntry entry in EntriesList)
            {
                
            }
            
            GetComponent<HidableUiElement>().Show();
        }

        private IEnumerator CreateCircleCoroutine(Vector2 anchoredPosition)
        {
            GraphPoint point = PointFactory.CreateUiGameObject(Content);
            yield return new WaitForEndOfFrame();
            point.SetPoint(anchoredPosition);
        }

        private void FillGraph(List<float> valueList)
        {
            float graphHeight = Content.GetComponent<RectTransform>().sizeDelta.y;
            float graphWidth = Content.GetComponent<RectTransform>().sizeDelta.x;
            float xSize = graphWidth / 33; //33 for 31 values a month plus spaces left and right.
            for (int i = 1; i < 31; i++) //31 for the month days.
            {
                float xPos = i * xSize;
                float yPos = (valueList[i] / 100) * graphHeight; //100 for the "by 100g on the food stats.
                StartCoroutine(CreateCircleCoroutine(new Vector2(xPos, yPos)));
            }
            
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WhateverDevs.Core.Runtime.Ui;

namespace ITCL.VisionNutricional.Runtime.DataBase
{
    public class DbTest : ActionOnButtonClick<DbTest>
    {
        [SerializeField] private TMP_Text Texto;

        [SerializeField] private HidableUiElement Hide;
        
        protected override void ButtonClicked()
        {
            StartCoroutine(ShowDbCoroutine());
        }

        private IEnumerator ShowDbCoroutine()
        {
            Texto.text = "USERS:\n";
            List<DB.User> users = DB.SelectAllUsers();
            foreach (DB.User user in users)
            {
                Texto.text += user.userName + "   " + user.email + "   " + user.password + "\n";
            }

            Texto.text += "\nFOODS:\n";
            List<DB.Food> foods = DB.SelectAllFoods();
            foreach (DB.Food food in foods)
            {
                Texto.text += food.foodName + "         " + food.foodCalories + "\n";
            }
            
            Texto.text += "\nHISTORIC:\n";
            List<DB.HistoricEntry> entries = DB.SelectAllEntries();
            foreach (DB.HistoricEntry entry in entries)
            {
                Texto.text += entry.userEmail + "   " + entry.foodName + "   " + entry.date + "\n";
            }
            
            Hide.Show();

            yield return new WaitForSeconds(5);

            Hide.Show(false);

            yield return new WaitForEndOfFrame();
        }
    }
}

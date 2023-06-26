using ITCL.VisionNutricional.Runtime.DataBase;
using TMPro;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.DependencyInjection;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime;
using WhateverDevs.Localization.Runtime.Ui;
using Zenject;

namespace ITCL.VisionNutricional.Runtime.Historic
{
    public class HistoricEntry : WhateverBehaviour<HistoricEntry>
    {
        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject] private ILocalizer localizer;
        
        /// <summary>
        /// Reference to the button subscribable.
        /// </summary>
        [HideInInspector] public EasySubscribableButton ButtonSus;
        
        /// <summary>
        /// Database info.
        /// </summary>
        private DB.HistoricEntry DbEntry;

        /// <summary>
        /// Reference to the food name localizer.
        /// </summary>
        [SerializeField] private LocalizedTextMeshPro FoodLocalizer;

        /// <summary>
        /// reference to the date field.
        /// </summary>
        [SerializeField] private TMP_Text Date;

        /// <summary>
        /// Gets the subscribable component.
        /// </summary>
        private void Start()
        {
            ButtonSus = GetComponent<EasySubscribableButton>();
        }
        
        /// <summary>
        /// Sets the entry data on its button.
        /// </summary>
        /// <param name="entry"></param>
        public void SetData(DB.HistoricEntry entry)
        {
            if (localizer["Foods/" + entry.foodName].Equals("Foods/" + entry.foodName)) FoodLocalizer.SetValue(entry.foodName);
            else FoodLocalizer.SetValue("Foods/" + entry.foodName);
            Date.text = entry.date;
        }

        // ReSharper disable once ClassNeverInstantiated.Global
        public class Factory : GameObjectFactory<HistoricEntry>
        {
        }
    }
}

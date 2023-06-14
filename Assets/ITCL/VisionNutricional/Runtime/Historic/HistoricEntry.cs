using ITCL.VisionNutricional.Runtime.DataBase;
using TMPro;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.DependencyInjection;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime.Ui;

namespace ITCL.VisionNutricional.Runtime.Historic
{
    public class HistoricEntry : WhateverBehaviour<HistoricEntry>
    {
        /// <summary>
        /// Reference to the button subscribable.
        /// </summary>
        [HideInInspector] public EasySubscribableButton ButtonSus;
        
        /// <summary>
        /// Database info.
        /// </summary>
        private DB.HistoricEntry DbEntry;

        /// <summary>
        /// Reference to the food field.
        /// </summary>
        [SerializeField] private LocalizedTextMeshPro Food;

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
            Food.SetValue("Foods/" + entry.foodName);
            Date.text = entry.date;
        }

        // ReSharper disable once ClassNeverInstantiated.Global
        public class Factory : GameObjectFactory<HistoricEntry>
        {
        }
    }
}

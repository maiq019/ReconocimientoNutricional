using TMPro;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.DependencyInjection;
using WhateverDevs.Core.Runtime.Ui;
using WhateverDevs.Localization.Runtime.Ui;

namespace ITCL.VisionNutricional.Runtime.Camera
{
    public class SelectableFood : WhateverBehaviour<SelectableFood>
    {
        /// <summary>
        /// Reference to the cloud receiver script.
        /// </summary>
        private CloudReceiver CloudRec;
        
        /// <summary>
        /// Reference to the button subscribable.
        /// </summary>
        [HideInInspector] public EasySubscribableButton ButtonSus;

        /// <summary>
        /// Reference to the localizer food name.
        /// </summary>
        [SerializeField] private TMP_Text TextFoodName;

        /// <summary>
        /// Name of the food represented on the selectable.
        /// </summary>
        private string foodName;
        
        /// <summary>
        /// Gets the subscribable component.
        /// </summary>
        private void Start()
        {
            ButtonSus = GetComponent<EasySubscribableButton>();
        }

        /// <summary>
        /// Sets the food name of the selectable.
        /// </summary>
        /// <param name="foodN">Name of the food.</param>
        protected internal void SetFood(string foodN)
        {
            foodName = foodN;
            TextFoodName.text = foodName;
        }
        
        // ReSharper disable once ClassNeverInstantiated.Global
        public class Factory : GameObjectFactory<SelectableFood>
        {
        }
    }
}

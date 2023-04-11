using System.Collections.Generic;
using WhateverDevs.Localization.Runtime;
using Zenject;

namespace ITCL.VisionNutricional.Runtime.UI
{
    /// <summary>
    /// Language input field (config) controller.
    /// </summary>
    public class EnumLanguageInputField : EnumValueHolder
    {
        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject]
        private ILocalizer localizer;

        /// <summary>
        /// List of values from languages.
        /// </summary>
        private List<string> enumList;
        
        /// <summary>
        /// Initialize the list of languages and sets current language.
        /// </summary>
        private void OnEnable()
        {
            enumList = localizer.GetAllLanguageIds();
            Set(localizer.GetCurrentLanguageId());
        }
        
        /// <summary>
        /// Getter for the index value.
        /// </summary>
        /// <returns>The index value of the language selection.</returns>
        public override int Get() => localizer.GetCurrentLanguageId();

        /// <summary>
        /// Setter for the index value.
        /// </summary>
        /// <param name="value">The new index value of the language selection.</param>
        public override void Set(int value)
        {
            if (value >= enumList.Count) value -= enumList.Count;
            if (value < 0) value += enumList.Count;
            localizer.SetLanguage(value);
        }
    }
}

using UnityEngine;
using WhateverDevs.Core.Runtime.Ui;

namespace ITCL.VisionNutricional.Runtime.UI
{
    /// <summary>
    /// Selection buttons for enum selection fields.
    /// </summary>
    public class SelectableEnumButton : ActionOnButtonClick<SelectableEnumButton>
    {
        /// <summary>
        /// Value modifier for the selection field.
        /// </summary>
        [SerializeField]
        private int Addition;

        /// <summary>
        /// Reference to the value holder from the selection field.
        /// </summary>
        [SerializeField]
        private EnumValueHolder ValueHolder;

        /// <summary>
        /// Current index value for the selection.
        /// </summary>
        private int CurrentValue
        {
            get => ValueHolder.Get();
            set => ValueHolder.Set(value);
        }

        /// <summary>
        /// Modifies the input field according to Addition value.
        /// </summary>
        protected override void ButtonClicked() => CurrentValue += Addition;
    }
}

using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.DependencyInjection;

namespace ITCL.VisionNutricional.Runtime.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class VerticalBar : WhateverBehaviour<VerticalBar>
    {
        /// <summary>
        /// Sets the transform values for the bar.
        /// </summary>
        /// <param name="width">Width of the bar.</param>
        /// <param name="offsize">Offsize from the width (to adjust the corners when forming a rectangle).</param>
        /// <param name="xPos">Horizontal position of the bar.</param>
        /// <param name="begin">Vertical position of one edge of the bar.</param>
        /// <param name="end">Vertical position of the other edge of the bar.</param>
        public void Set(float width, float offsize, float xPos, float begin, float end)
        {
            RectTransform rect = this.GetComponent<RectTransform>();
            
            rect.sizeDelta = new Vector2(width, offsize);
            rect.anchorMin = new Vector2(xPos,begin);
            rect.anchorMax = new Vector2(xPos,end);
        }
        
        // ReSharper disable once ClassNeverInstantiated.Global
        public class Factory : GameObjectFactory<VerticalBar>
        {
        }
    }
}

using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.DependencyInjection;

namespace ITCL.VisionNutricional.Runtime.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class HorizontalBar : WhateverBehaviour<HorizontalBar>
    {
        /// <summary>
        /// Sets the transform values for the bar.
        /// </summary>
        /// <param name="width">Width of the bar.</param>
        /// <param name="offsize">Offsize from the width (to adjust the corners when forming a rectangle).</param>
        /// <param name="yPos">Vertical position of the bar.</param>
        /// <param name="begin">Horizontal position of one edge of the bar.</param>
        /// <param name="end">Horizontal position of the other edge of the bar.</param>
        public void Set(float width, float offsize, float yPos, float begin, float end)
        {
            RectTransform rect = this.GetComponent<RectTransform>();
        
            rect.sizeDelta = new Vector2(offsize, width);
            rect.anchorMin = new Vector2(begin,yPos);
            rect.anchorMax = new Vector2(end,yPos);
        }
    
        // ReSharper disable once ClassNeverInstantiated.Global
        public class Factory : GameObjectFactory<HorizontalBar>
        {
        }
        
    }
}

using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.DependencyInjection;

namespace ITCL.VisionNutricional.Runtime.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class HorizontalBar : WhateverBehaviour<HorizontalBar>
    {
    
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

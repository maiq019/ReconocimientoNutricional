using System;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.DependencyInjection;

namespace ITCL.VisionNutricional.Runtime.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class VerticalBar : WhateverBehaviour<VerticalBar>
    {
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

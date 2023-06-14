using System;
using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.DependencyInjection;

namespace ITCL.VisionNutricional.Runtime.Historic
{
    public class GraphPoint : WhateverBehaviour<GraphPoint>
    {
        /// <summary>
        /// Reference to own rect transform.
        /// </summary>
        private RectTransform RectTransform;

        /// <summary>
        /// Sets static values of the point.
        /// </summary>
        private void Awake()
        { 
            RectTransform = GetComponent<RectTransform>();
            RectTransform.sizeDelta = new Vector2(30, 30);
            RectTransform.anchorMin = new Vector2(0, 0);
            RectTransform.anchorMax = new Vector2(0, 0);
        }

        /// <summary>
        /// Sets the position of the point.
        /// </summary>
        /// <param name="anchoredPosition">x and Y of the point in the graph container.</param>
        protected internal void SetPoint(Vector2 anchoredPosition)
        {
            RectTransform.anchoredPosition = anchoredPosition;
        }
        
        // ReSharper disable once ClassNeverInstantiated.Global
        public class Factory : GameObjectFactory<GraphPoint>
        {
        }
    }
}

using System;
using UnityEngine;

namespace Proxima
{
    internal class ProximaReadme : ScriptableObject
    {
        public Texture2D icon = null;
        public Section[] sections = null;

        [Serializable]
        internal class Section
        {
            public string heading = null;
            public string text = null;
            public string linkText = null;
            public string url = null;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace ITCL.VisionNutricional.Runtime.DataBase
{
    /// <summary>
    /// Scriptable class to collect all the foods.
    /// </summary>
    public class ScriptableFood : ScriptableObject
    {
        public List<DB.Food> Foods = new();
    }
}

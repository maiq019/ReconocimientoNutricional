using UnityEngine;

namespace ITCL.VisionNutricional.Runtime.UI
{
    /// <summary>
    /// Abstract holder of index values for each enum selection field.
    /// </summary>
    public abstract class EnumValueHolder : MonoBehaviour
    {
        /// <summary>
        /// Getter for the hold index value.
        /// </summary>
        /// <returns>The index value of the enum selection.</returns>
        public abstract int Get();

        /// <summary>
        /// Setter for the index value.
        /// </summary>
        /// <param name="value">The new index value of the enum selection.</param>
        public abstract void Set(int value);
    }
}

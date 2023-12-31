using UnityEngine;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Build;
using WhateverDevs.Localization.Runtime.Ui;

namespace ITCL.VisionNutricional.Runtime.UI
{
    /// <summary>
    /// Info display manager for the footer prefab.
    /// </summary>
    public class FooterDisplay : WhateverBehaviour<FooterDisplay>
    {
        /// <summary>
        /// Reference to the version control scriptable.
        /// </summary>
        [SerializeField] private Version Version;

        /// <summary>
        /// Version text on the bottom of the main menu window.
        /// </summary>
        [SerializeField] private LocalizedTextMeshPro VersionText;
        
        private void OnEnable() => VersionText.SetValue("Common/Menu/Version", false, Version.ToString(VersionDisplayMode.Short));
    }
}

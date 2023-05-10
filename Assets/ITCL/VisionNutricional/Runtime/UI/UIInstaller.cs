using ITCL.VisionNutricional.Runtime.Historic;
using UnityEngine;
using Zenject;

namespace ITCL.VisionNutricional.Runtime.UI
{
    /// <summary>
    /// Installer for the UI elements in the rehab project.
    /// </summary>
    [CreateAssetMenu(menuName = "ITCL/VisionNutricional/UI/UIInstaller", fileName = "UIInstaller")]
    public class UIInstaller : ScriptableObjectInstaller
    {
        /// <summary>
        /// Reference to the exercises manager.
        /// </summary>
        [SerializeField] private HistoricEntryManager Entry;
        
        /// <summary>
        /// Reference injections.
        /// </summary>
        public override void InstallBindings()
        {
            Container.BindFactory<HistoricEntryManager,
                    HistoricEntryManager.Factory>()
                .FromComponentInNewPrefab(Entry)
                .AsSingle()
                .Lazy();
        }
    }
}

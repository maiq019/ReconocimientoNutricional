using ITCL.VisionNutricional.Runtime.Camera;
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
        [SerializeField] private HistoricEntry Entry;

        [SerializeField] private VerticalBar VBar;
        
        [SerializeField] private HorizontalBar HBar;

        [SerializeField] private SelectableFood SelectableF;

        [SerializeField] private GraphPoint GPoint;
        
        /// <summary>
        /// Reference injections.
        /// </summary>
        public override void InstallBindings()
        {
            Container.BindFactory<HistoricEntry,
                    HistoricEntry.Factory>()
                .FromComponentInNewPrefab(Entry)
                .AsSingle()
                .Lazy();
            
            Container.BindFactory<VerticalBar,
                    VerticalBar.Factory>()
                .FromComponentInNewPrefab(VBar)
                .AsSingle()
                .Lazy();
            
            Container.BindFactory<HorizontalBar,
                    HorizontalBar.Factory>()
                .FromComponentInNewPrefab(HBar)
                .AsSingle()
                .Lazy();
            
            Container.BindFactory<SelectableFood,
                    SelectableFood.Factory>()
                .FromComponentInNewPrefab(SelectableF)
                .AsSingle()
                .Lazy();
            
            Container.BindFactory<GraphPoint,
                    GraphPoint.Factory>()
                .FromComponentInNewPrefab(GPoint)
                .AsSingle()
                .Lazy();
        }
    }
}

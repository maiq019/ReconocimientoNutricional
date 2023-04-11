using UnityEngine;
using WhateverDevs.Core.Runtime.Configuration;
using WhateverDevs.Core.Runtime.Persistence;

namespace ITCL.VisionNutricional.Runtime.Configuration
{
    /// <summary>
    /// Extenject installer for the Configuration Manager.
    /// We inject different persisters depending on if we want the user to have a local persistent copy of the config.
    /// </summary>
    [CreateAssetMenu(menuName = "ITCL/VisionNutricional/DI/ConfigurationManager",
        fileName = "ConfigurationManagerInstaller")]
    public class ConfigurationManagerInstaller : WhateverDevs.Core.Runtime.Configuration.ConfigurationManagerInstaller
    {
        /// <summary>
        /// Set the persisters for each of the configs, then call the base injection.
        /// </summary>
        public override void InstallBindings()
        {
            // All configurations should the persistent data path.
            Container.Bind<IPersister>()
                .To<ConfigurationJsonFilePersisterOnPersistentDataPath>()
                .AsCached()
                .WhenInjectedInto<IConfiguration>()
                .Lazy();

            base.InstallBindings();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace DryIoc.Extensions.Modularity
{
    /// <summary>
    /// Dry IoC module extensions
    /// </summary>
    public static class DryIocModuleExtensions
    {
        #region Collection of modules

        /// <summary>
        /// Add collection of modules from configuration settings.
        /// </summary>
        /// <param name="container">Instance of DryIoc container.</param>
        /// <param name="configuration">Configuration section where defined collection of <see cref="ModuleInfo"/></param>
        /// <returns>DryIoc container</returns>
        public static IContainer AddModules(this IContainer container, IConfiguration configuration)
        {
            var activeModules = configuration.Get<ModuleInfo[]>()
                ?.Where(i => i.Enabled)
                .ToArray();

            if (activeModules != null && activeModules.Any())
            {
                return AddModules(container, activeModules);
            }

            return container;
        }

        /// <summary>
        /// Add collection of modules manually.
        /// </summary>
        /// <param name="container">Instance of DryIoc container.</param>
        /// <param name="modulesInfo">Instance of collection of the <see cref="IModuleInfo"/></param>
        /// <returns>DryIoc container</returns>
        public static IContainer AddModules(this IContainer container, IEnumerable<IModuleInfo> modulesInfo)
        {
            var modulesInfoArray = modulesInfo as IModuleInfo[] ?? modulesInfo.ToArray();
            var activeModules = modulesInfoArray.Where(i => i.Enabled).ToArray();

            if (!activeModules.Any())
                return container;

            foreach (var moduleInfo in activeModules)
            {
                container.AddModule(moduleInfo);
            }
            
            return container;
        }

        #endregion

        #region Single module

        /// <summary>
        /// Add module using type of <typeparamref name="TModule"/>
        /// </summary>
        /// <remarks>
        /// Added modules automatically enabled by default.
        /// </remarks>
        /// <typeparam name="TModule">Type of module which implemented <see cref="IModule"/></typeparam>
        /// <param name="container">Instance of DryIoc container.</param>
        /// <returns>DryIoc container</returns>
        public static IContainer AddModule<TModule>(this IContainer container) where TModule: IModule
        {
            var module = Activator.CreateInstance<TModule>();

            // if module already registered then skip it
            if (container.IsRegistered<TModule>(module.Name))
                return container;

            module.Enabled = true;
            module.RegisterTypes(container);
            container.RegisterInstance(module, serviceKey: module.Name);
            return container;
        }

        /// <summary>
        /// Add module from configuration settings.
        /// </summary>
        /// <param name="container">Instance of DryIoc container.</param>
        /// <param name="configuration">Configuration section where defined <see cref="ModuleInfo"/></param>
        /// <returns>DryIoc container</returns>
        public static IContainer AddModule(this IContainer container, IConfiguration configuration)
        {
            var moduleInfo = configuration.Get<ModuleInfo>();
            return AddModule(container, moduleInfo);
        }

        /// <summary>
        /// Add module manually.
        /// </summary>
        /// <param name="container">Instance of DryIoc container.</param>
        /// <param name="moduleInfo">Instance of <see cref="IModuleInfo"/></param>
        /// <returns>DryIoc container</returns>
        public static IContainer AddModule(this IContainer container, IModuleInfo moduleInfo)
        {
            if (moduleInfo == null || !moduleInfo.Enabled)
                return container;

            var moduleAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(i => string.Equals(i.GetName().Name, moduleInfo.AssemblyName, StringComparison.CurrentCultureIgnoreCase));

            if (moduleAssembly == null)
            {
                moduleAssembly = AppDomain.CurrentDomain.Load(new AssemblyName { Name = moduleInfo.AssemblyName});
            }

            var moduleType = moduleAssembly.GetTypes()
                .FirstOrDefault(i => i.GetInterfaces().Contains(typeof(IModule)));

            if (moduleType == null) 
                return container;

            var moduleInstance = Activator.CreateInstance(moduleType);

            if (!(moduleInstance is IModule module) || 
                !string.Equals(module.Name, moduleInfo.Name, StringComparison.CurrentCultureIgnoreCase)) 
                return container;

            // if module already registered then skip it
            if (container.IsRegistered(moduleType, module.Name))
                return container;
            
            module.Enabled = moduleInfo.Enabled;
            module.AssemblyName = moduleInfo.AssemblyName;
            module.RegisterTypes(container);
            container.RegisterInstance(module, serviceKey: module.Name);

            return container;
        }

        #endregion
    }
}
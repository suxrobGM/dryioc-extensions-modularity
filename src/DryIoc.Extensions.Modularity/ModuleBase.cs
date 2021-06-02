using System.Reflection;

namespace DryIoc.Extensions.Modularity
{
    /// <summary>
    /// Base abstract class of module
    /// </summary>
    /// <typeparam name="TModule">Type of module which implements <see cref="IModule"/></typeparam>
    public abstract class ModuleBase<TModule> : IModule where TModule : IModule
    {
        protected ModuleBase()
        {
            AssemblyName = Assembly.GetAssembly(typeof(TModule)).GetName().Name;
            Name = typeof(TModule).Name;
        }

        public string Name { get; set; }
        public string AssemblyName { get; set; }
        public bool Enabled { get; set; }

        public abstract void RegisterTypes(IContainer container);
    }
}

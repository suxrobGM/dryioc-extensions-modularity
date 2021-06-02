namespace DryIoc.Extensions.Modularity
{
    /// <summary>
    /// Defines the contract for the modules deployed in the application.
    /// </summary>
    public interface IModule : IModuleInfo
    {
        /// <summary>
        /// Used to register types with the DryIoc container.
        /// </summary>
        void RegisterTypes(IContainer container);
    }
}
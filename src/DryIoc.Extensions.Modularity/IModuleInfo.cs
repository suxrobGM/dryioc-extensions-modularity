namespace DryIoc.Extensions.Modularity
{
    /// <summary>
    /// Set of properties for each Module.
    /// </summary>
    public interface IModuleInfo 
    {
        /// <summary>
        /// The name of the module.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Simple name of assembly where defined module, and name should be file extension.
        /// </summary>
        string AssemblyName { get; set; }

        /// <summary>
        /// Determines that the module is enabled.
        /// </summary>
        bool Enabled { get; set; }
    }
}

namespace DryIoc.Extensions.Modularity
{
    /// <summary>
    /// Define information about the module.
    /// </summary>
    public class ModuleInfo : IModuleInfo
    {
        public string Name { get; set; }
        public string AssemblyName { get; set; }
        public bool Enabled { get; set; }
    }
}
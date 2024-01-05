namespace Frank.PluginFramework;

public class PluginContext : IPluginContext
{
    public PluginContext(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public IServiceProvider ServiceProvider { get; }
}
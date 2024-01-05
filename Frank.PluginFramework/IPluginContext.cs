namespace Frank.PluginFramework;

public interface IPluginContext
{
    IServiceProvider ServiceProvider { get; }
}
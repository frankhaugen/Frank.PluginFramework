namespace Frank.PluginFramework;

public interface IPlugin
{
    /// <summary>
    /// Asynchronously executes the plugin with the given context.
    /// </summary>
    /// <param name="context">The context for the plugin execution.</param>
    /// <returns>A task that represents the asynchronous execution of the plugin. The task result contains a boolean value indicating success or failure.</returns>
    Task<bool> ExecuteAsync(IPluginContext context);
}
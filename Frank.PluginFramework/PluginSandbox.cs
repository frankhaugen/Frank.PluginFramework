using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;

namespace Frank.PluginFramework;

public class PluginSandbox : IDisposable
{
    private AssemblyLoadContext _loadContext;

    public PluginSandbox()
    {
        _loadContext = new AssemblyLoadContext("PluginSandbox", isCollectible: true);
    }

    public Task<PluginExecutionResult> ExecutePluginAsync(FileInfo assemblyFile, IServiceProvider serviceProvider)
    {
        var assembly = _loadContext.LoadFromAssemblyPath(assemblyFile.FullName);
        return ExecutePluginAsync(assembly, serviceProvider);
    }
    
    public async Task<PluginExecutionResult> ExecutePluginAsync(Stream assemblyStream, IServiceProvider serviceProvider)
    {
        var assembly = _loadContext.LoadFromStream(assemblyStream);
        return await ExecutePluginAsync(assembly, serviceProvider);
    }
    
    public async Task<PluginExecutionResult> ExecutePluginAsync(Assembly assembly, IServiceProvider serviceProvider)
    {
        var pluginType = assembly.GetTypes().FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t));
        if (pluginType is null)
        {
            return new PluginExecutionResult(false, new Exception("No plugin type found in assembly"), TimeSpan.Zero);
        }

        var plugin = (IPlugin)Activator.CreateInstance(pluginType);
        var context = new PluginContext(serviceProvider);
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var success = await plugin.ExecuteAsync(context);
            return new PluginExecutionResult(success, null, stopwatch.Elapsed);
        }
        catch (Exception e)
        {
            return new PluginExecutionResult(false, e, stopwatch.Elapsed);
        }
    }

    public void Dispose()
    {
        _loadContext.Unload();
    }
}
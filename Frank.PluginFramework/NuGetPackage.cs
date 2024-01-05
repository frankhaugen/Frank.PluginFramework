namespace Frank.PluginFramework;

public class NuGetPackage
{
    public string Id { get; set; }
    public string Version { get; set; }

    public FileStreamCollection FileStreamCollection { get; private set; } = new();
}
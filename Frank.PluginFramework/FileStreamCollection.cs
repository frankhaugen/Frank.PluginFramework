namespace Frank.PluginFramework;

public class FileStreamCollection : HashSet<KeyValuePair<string, MemoryStream>>, IDisposable
{
    public void Add(string filename, MemoryStream stream) => Add(new KeyValuePair<string, MemoryStream>(filename, stream));

    public void Dispose()
    {
        foreach (var kvp in this)
        {
            kvp.Value.Dispose();
            Remove(kvp);
        }
    }
}
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace Frank.PluginFramework;

public class NuGetPackageClient
{
    private const string PackagesDirectory = "./packages";
    private const string PackageSourceUrl = "https://api.nuget.org/v3/index.json";
    private static readonly IEnumerable<Lazy<INuGetResourceProvider>> Providers = Repository.Provider.GetCoreV3();
    private static readonly SourceRepository SourceRepository = new(new PackageSource(PackageSourceUrl), Providers);
    private static readonly SourceCacheContext CacheContext = new();
    private static readonly PackageDownloadContext DownloadContext = new(CacheContext);
    private readonly ILogger _nuGetLogger = NullLogger.Instance;

    public async Task<IEnumerable<IPackageSearchMetadata>> GetMetaDataAsync(string packageId, CancellationToken cancellationToken, bool includePrerelease = false, bool includeUnlisted = false)
    {
        var packageMetadataResource = await SourceRepository.GetResourceAsync<PackageMetadataResource>(cancellationToken);
        return await packageMetadataResource.GetMetadataAsync(packageId, includePrerelease, includeUnlisted, CacheContext, _nuGetLogger, cancellationToken);
    }

    public async Task<IPackageSearchMetadata> GetMetaDataAsync(PackageIdentity packageIdentity, CancellationToken cancellationToken)
    {
        var packageMetadataResource = await SourceRepository.GetResourceAsync<PackageMetadataResource>(cancellationToken);
        return await packageMetadataResource.GetMetadataAsync(packageIdentity, CacheContext, _nuGetLogger, cancellationToken);
    }
    
    public async Task<DownloadResourceResult> DownloadPackageAsync(PackageIdentity packageIdentity, CancellationToken cancellationToken)
    {
        var packageDownloadResource = await SourceRepository.GetResourceAsync<DownloadResource>(cancellationToken);
        var result = await packageDownloadResource.GetDownloadResourceResultAsync(packageIdentity, DownloadContext, PackagesDirectory, _nuGetLogger, cancellationToken);
        
        return result;
    }
    
    public async Task<DownloadResourceResult> DownloadPackageAsync(PackageIdentity packageIdentity, DirectoryInfo directory, CancellationToken cancellationToken)
    {
        var packageDownloadResource = await SourceRepository.GetResourceAsync<DownloadResource>(cancellationToken);
        var result = await packageDownloadResource.GetDownloadResourceResultAsync(packageIdentity, DownloadContext, directory.FullName, _nuGetLogger, cancellationToken);
        
        return result;
    }
    
    public async Task DownloadPackageAndDependenciesAsync(PackageIdentity packageIdentity, DirectoryInfo directory, CancellationToken cancellationToken)
    {
        var downloadedPackages = new HashSet<PackageIdentity>();
        await DownloadPackageRecursive(packageIdentity, downloadedPackages, cancellationToken, directory);
    }

    private async Task DownloadPackageRecursive(PackageIdentity packageIdentity, HashSet<PackageIdentity> downloadedPackages, CancellationToken cancellationToken, DirectoryInfo directory)
    {
        if (downloadedPackages.Contains(packageIdentity))
        {
            return; // Package already downloaded
        }
        var package = await DownloadPackageAsync(packageIdentity, directory, cancellationToken);
        downloadedPackages.Add(packageIdentity);
        var dependencies = await package.PackageReader.GetPackageDependenciesAsync(cancellationToken);
        foreach (var dependencySet in dependencies)
        {
            foreach (var packageDependency in dependencySet.Packages)
            {
                var resolvedVersion = NuGetVersion.Parse(packageDependency.VersionRange.OriginalString);
                var dependencyIdentity = new PackageIdentity(packageDependency.Id, resolvedVersion);
                await DownloadPackageRecursive(dependencyIdentity, downloadedPackages, cancellationToken, directory);
            }
        }
    }
}
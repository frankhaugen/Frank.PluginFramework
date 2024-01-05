using System.Reflection;
using Frank.PluginFramework;
using JetBrains.Annotations;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Versioning;
using Xunit.Abstractions;

namespace Frank.PluginFramework.Tests;

[TestSubject(typeof(NuGetPackageClient))]
public class NuGetPackageClientTests
{
    private readonly ITestOutputHelper _outputHelper;

    public NuGetPackageClientTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }
    
    [Fact]
    public async Task TestDownloadPackageAsync()
    {
        var directory = new DirectoryInfo("D:/temp/nugets");
        
        if (!directory.Exists) directory.Create();
        
        var client = new NuGetPackageClient();
        var packageIdentity = new PackageIdentity("Frank.PulseFlow", NuGetVersion.Parse("1.0.0"));
        // var package = await client.DownloadPackageAsync(packageIdentity, CancellationToken.None);
        // var dependencies = package.PackageReader.GetPackageDependencies();
        await client.DownloadPackageAndDependenciesAsync(
            packageIdentity, 
            directory, CancellationToken.None);
        
        var files = directory.GetFiles("*.dll", SearchOption.AllDirectories);
        
        _outputHelper.WriteCSharp(files.Select(x => x.FullName));
    }
}
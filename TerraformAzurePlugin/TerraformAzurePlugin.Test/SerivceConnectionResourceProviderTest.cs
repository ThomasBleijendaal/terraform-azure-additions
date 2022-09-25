using Microsoft.Extensions.DependencyInjection;
using TerraformPluginDotNet;
using TerraformPluginDotNet.ResourceProvider;
using TerraformPluginDotNet.Testing;

namespace TerraformAzurePlugin.Test;

[TestFixture(Category = "Functional", Explicit = true)]
public class SerivceConnectionResourceProviderTest
{
    private const string ProviderName = "azureadditions";

    private TerraformTestHost _host;

    [OneTimeSetUp]
    public void Setup()
    {
        _host = new TerraformTestHost("C:\\Projects\\_tools\\terraform.exe");
        _host.Start($"thomas-ict.nl/azure/{ProviderName}", Configure);
    }

    [OneTimeTearDown]
    public async Task TearDownAsync()
    {
        await _host.DisposeAsync();
    }

    private void Configure(IServiceCollection services, IResourceRegistryContext registryContext)
    {
        services.AddSingleton<AzureConfigurator>();
        services.AddTerraformProviderConfigurator<AzureConfiguration, AzureConfigurator>();
        services.AddSingleton<IResourceProvider<ServiceConnectionResource>, ServiceConnectionResourceProvider>();
        registryContext.RegisterResource<ServiceConnectionResource>($"{ProviderName}_serviceconnection");
    }

    [Test]
    public async Task TestCreateServiceConnectionResourceAsync()
    {
        using var terraform = await _host.CreateTerraformTestInstanceAsync(ProviderName);

        var resourcePath = Path.Combine(terraform.WorkDir, "file.tf");
        
        await File.WriteAllTextAsync(resourcePath, $@"
resource ""{ProviderName}_serviceconnection"" ""conn"" {{
    id = ""{Guid.NewGuid()}""
}}
");

        var planOutput = await terraform.PlanWithOutputAsync();
        var applyOutput = await terraform.ApplyAsync();
    }
}

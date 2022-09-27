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
        services.AddHttpClient<IResourceProvider<ServiceConnectionResource>, ServiceConnectionResourceProvider>();
        registryContext.RegisterResource<ServiceConnectionResource>($"{ProviderName}_serviceconnection");
    }

    [Test]
    public async Task TestCreateServiceConnectionResourceAsync()
    {
        using var terraform = await _host.CreateTerraformTestInstanceAsync(ProviderName, configure: false);

        var resourcePath = Path.Combine(terraform.WorkDir, "file.tf");

        await File.WriteAllTextAsync(resourcePath, $$"""
            provider "azureadditions" {
              org_service_url       = "{{Environment.GetEnvironmentVariable("ORGURL")}}"
              personal_access_token = "{{Environment.GetEnvironmentVariable("PAT")}}"
            }

            terraform {
              required_providers {
                {{ProviderName}} = {
                  source = "example.com/example/{{ProviderName}}"
                  version = "1.0.0"
                }
              }
            }

            resource "{{ProviderName}}_serviceconnection" "conn" {
                id = "{{Environment.GetEnvironmentVariable("SCID")}}"
                project_id = "{{Environment.GetEnvironmentVariable("PID")}}"
            }
            """);

        var planOutput = await terraform.PlanWithOutputAsync();

        Console.WriteLine(planOutput);

        var applyOutput = await terraform.ApplyAsync();

        Console.WriteLine(applyOutput);
    }
}

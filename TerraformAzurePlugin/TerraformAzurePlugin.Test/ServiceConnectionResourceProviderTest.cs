using Microsoft.Extensions.DependencyInjection;
using TerraformPluginDotNet;
using TerraformPluginDotNet.ResourceProvider;
using TerraformPluginDotNet.Testing;

namespace TerraformAzurePlugin.Test;

[TestFixture(Category = "Functional", Explicit = true)]
public class ServiceConnectionResourceProviderTest
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
        services.AddHttpClient<IDataSourceProvider<ServiceConnectionResource>, ServiceConnectionResourceProvider>();
        registryContext.RegisterDataSource<ServiceConnectionResource>($"{ProviderName}_serviceconnection");
    }

    [Test]
    public async Task TestCreateServiceConnectionResourceAsync()
    {
        using var terraform = await _host.CreateTerraformTestInstanceAsync(ProviderName, configureProvider: false);

        var resourcePath = Path.Combine(terraform.WorkDir, "file.tf");

        await File.WriteAllTextAsync(resourcePath, $$"""
            provider "azureadditions" {
              org_service_url       = "{{Environment.GetEnvironmentVariable("ORGURL")}}"
              personal_access_token = "{{Environment.GetEnvironmentVariable("PAT")}}"
            }

            data "{{ProviderName}}_serviceconnection" "conn" {
                service_connection_id = "{{Environment.GetEnvironmentVariable("SCID")}}"
                project_id = "{{Environment.GetEnvironmentVariable("PID")}}"
            }
            """);

        var planOutput = await terraform.PlanWithOutputAsync();

        Console.WriteLine(planOutput);

        var applyOutput = await terraform.ApplyAsync();

        Console.WriteLine(applyOutput);
    }

    [Test]
    public async Task TestNoOpUpdateServiceConnectionResourceAsync()
    {
        using var terraform = await _host.CreateTerraformTestInstanceAsync(ProviderName, configureProvider: false, configureTerraform: true);

        await File.WriteAllTextAsync(Path.Combine(terraform.WorkDir, "file.tf"), $$"""
            provider "azureadditions" {
              org_service_url       = "{{Environment.GetEnvironmentVariable("ORGURL")}}"
              personal_access_token = "{{Environment.GetEnvironmentVariable("PAT")}}"
            }

            data "{{ProviderName}}_serviceconnection" "conn" {
                service_connection_id = "{{Environment.GetEnvironmentVariable("SCID")}}"
                project_id = "{{Environment.GetEnvironmentVariable("PID")}}"
            }
            """);

        await File.WriteAllTextAsync(Path.Combine(terraform.WorkDir, "terraform.tfstate"), $$"""
        {
          "version": 4,
          "terraform_version": "1.2.9",
          "serial": 1,
          "lineage": "f8244b30-6b8e-1f3a-63fa-b535791ceb2e",
          "outputs": {},
          "resources": [
            {
              "mode": "data",
              "type": "azureadditions_serviceconnection",
              "name": "conn",
              "provider": "provider[\"example.com/example/azureadditions\"]",
              "instances": [
                {
                  "schema_version": 1,
                  "attributes": {
                    "id": "{{Environment.GetEnvironmentVariable("PID")}}/{{Environment.GetEnvironmentVariable("SCID")}}",
                    "project_id": "{{Environment.GetEnvironmentVariable("PID")}}",
                    "service_connection_id": "{{Environment.GetEnvironmentVariable("SCID")}}",
                    "service_principal_application_id": "{{Environment.GetEnvironmentVariable("AID")}}"
                  },
                  "sensitive_attributes": []
                }
              ]
            }
          ]
        }

        """);

        var planOutput = await terraform.PlanWithOutputAsync();

        Assert.That(planOutput.ResourceChanges, Is.Null);
    }
}



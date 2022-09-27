using Serilog;
using TerraformAzurePlugin;
using TerraformPluginDotNet;
using TerraformPluginDotNet.ResourceProvider;

/*
 * Terraform plugin todo's
 * - Create own Required attribute
 * - ForceNew attribute?
 * - Nullables on interfaces
 * - Test for missing schema
 * - Move logging to ILogger
 * - Configure in CreateTerraformTestInstanceAsync should still configure terraform
 */

await TerraformPluginHost.RunAsync(args, "thomas-ict.nl/azure/azureadditions", (services, registry) =>
{
    services.AddSingleton<AzureConfigurator>();
    services.AddTerraformProviderConfigurator<AzureConfiguration, AzureConfigurator>();
    services.AddHttpClient<IResourceProvider<ServiceConnectionResource>, ServiceConnectionResourceProvider>();
    registry.RegisterResource<ServiceConnectionResource>("azureadditions_serviceconnection");
});

using TerraformAzurePlugin;
using TerraformPluginDotNet;
using TerraformPluginDotNet.ResourceProvider;

/*
 * Terraform plugin todo's
 * V Create own Required attribute
 * - ForceNew attribute?
 * V Nullables on interfaces
 * V Test for missing schema
 * - Move logging to ILogger
 * V Configure in CreateTerraformTestInstanceAsync should still configure terraform
 * 
 * Provider todo's
 * - Apply keeps applying some known after apply..
 * 
 */

await TerraformPluginHost.RunAsync(args, "thomas-ict.nl/azure/azureadditions", (services, registry) =>
{
    services.AddSingleton<AzureConfigurator>();
    services.AddTerraformProviderConfigurator<AzureConfiguration, AzureConfigurator>();
    services.AddHttpClient<IResourceProvider<ServiceConnectionResource>, ServiceConnectionResourceProvider>();
    registry.RegisterResource<ServiceConnectionResource>("azureadditions_serviceconnection");
});

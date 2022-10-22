using TerraformAzurePlugin;
using TerraformPluginDotNet;
using TerraformPluginDotNet.ResourceProvider;

await TerraformPluginHost.RunAsync(args, "thomas-ict.nl/azure/azureadditions", (services, registry) =>
{
    services.AddSingleton<AzureConfigurator>();
    services.AddTerraformProviderConfigurator<AzureConfiguration, AzureConfigurator>();
    services.AddHttpClient<IDataSourceProvider<ServiceConnectionResource>, ServiceConnectionResourceProvider>();
    registry.RegisterDataSource<ServiceConnectionResource>("azureadditions_serviceconnection");
});

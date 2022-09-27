using TerraformPluginDotNet.ProviderConfig;

namespace TerraformAzurePlugin;

public class AzureConfigurator : IProviderConfigurator<AzureConfiguration>
{
    private AzureConfiguration? _config;

    public Task ConfigureAsync(AzureConfiguration config)
    {
        _config = config;

        return Task.CompletedTask;
    }

    public AzureConfiguration Config => _config ?? throw new InvalidOperationException("Read from configuration before it is configured");
}


using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using MessagePack;
using Serilog;
using TerraformPluginDotNet;
using TerraformPluginDotNet.ProviderConfig;
using TerraformPluginDotNet.ResourceProvider;
using TerraformPluginDotNet.Resources;
using TerraformPluginDotNet.Serialization;
using Key = MessagePack.KeyAttribute;

/*
 * Terraform plugin todo's
 * - Create own Required attribute
 * - ForceNew attribute?
 * - Nullables on interfaces
 * - Test for missing schema
 * - Move logging to ILogger
 */

await TerraformPluginHost.RunAsync(args, "thomas-ict.nl/azure/azureadditions", (services, registry) =>
{
    var config = new LoggerConfiguration();

    Log.Logger = config.WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

    Log.Logger.Information("HI");

    services.AddSingleton<AzureConfigurator>();
    Log.Logger.Information("HI2");
    services.AddTerraformProviderConfigurator<AzureConfiguration, AzureConfigurator>();
    Log.Logger.Information("HI3");
    services.AddSingleton<IResourceProvider<ServiceConnectionResource>, ServiceConnectionResourceProvider>();
    Log.Logger.Information("HI4");
    registry.RegisterResource<ServiceConnectionResource>("azureadditions_serviceconnection");
    Log.Logger.Information("HI5");

    services.AddSingleton<IDynamicValueSerializer, DebugDefaultDynamicValueSerializer>();
});

public class DebugDefaultDynamicValueSerializer : IDynamicValueSerializer
{
    public T DeserializeJson<T>(ReadOnlyMemory<byte> value)
    {
        return JsonSerializer.Deserialize<T>(value.Span, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public T DeserializeMsgPack<T>(ReadOnlyMemory<byte> value)
    {
        return MessagePackSerializer.Deserialize<T>(value);
    }

    byte[] IDynamicValueSerializer.SerializeMsgPack<T>(T value)
    {
        return MessagePackSerializer.Serialize(value);
    }
}


[SchemaVersion(1)]
[MessagePackObject]
public class AzureConfiguration
{
    [Key("personal_access_token")]
    [Description("Personal access token")]

    public string PersonalAccessToken { get; set; } = null!;
}

public class AzureConfigurator : IProviderConfigurator<AzureConfiguration>
{
    private AzureConfiguration? _config;

    public Task ConfigureAsync(AzureConfiguration config)
    {
        _config = config;

        Log.Logger.Information("CONFIG");
        return Task.CompletedTask;
    }

    public AzureConfiguration Config => _config ?? throw new InvalidOperationException("Read from configuration before it is configured");
}

[SchemaVersion(1)]
[MessagePackObject]
public class ServiceConnectionResource
{
    [Key("id")]
    [Description("ID of the service connection in Azure DevOps")]
    [Required]
    [MessagePackFormatter(typeof(ComputedValueFormatter))]
    public string Id { get; set; } = null!;

    [Key("serviceprincipalid")]
    [Description("ID of the service principal assigned to the service connection")]
    [Computed]
    [MessagePackFormatter(typeof(ComputedValueFormatter))]
    public string? ServicePrincipalId { get; set; } = null!;
}

public class ServiceConnectionResourceProvider : IResourceProvider<ServiceConnectionResource>
{
    private readonly AzureConfigurator _azureConfigurator;

    public ServiceConnectionResourceProvider(
        AzureConfigurator azureConfigurator)
    {
        Log.Logger.Information("P");
        _azureConfigurator = azureConfigurator;
    }

    public Task<ServiceConnectionResource> CreateAsync(ServiceConnectionResource planned)
    {
        Log.Logger.Information("C");
        planned.ServicePrincipalId = Guid.NewGuid().ToString();

        return Task.FromResult(planned);
    }

    public Task DeleteAsync(ServiceConnectionResource resource)
    {
        Log.Logger.Information("D");
        return Task.CompletedTask;
    }

    public Task<ServiceConnectionResource> PlanAsync(ServiceConnectionResource? prior, ServiceConnectionResource proposed)
    {
        Log.Logger.Information("PLAN");
        var resource = new ServiceConnectionResource
        {
            Id = proposed.Id
        };

        if (prior?.Id != proposed.Id)
        {
            resource.ServicePrincipalId = null;
        }

        return Task.FromResult(resource);
    }

    public Task<ServiceConnectionResource> ReadAsync(ServiceConnectionResource resource)
    {
        Log.Logger.Information("R");
        resource.ServicePrincipalId = Guid.NewGuid().ToString();

        return Task.FromResult(resource);

    }

    public Task<ServiceConnectionResource> UpdateAsync(ServiceConnectionResource? prior, ServiceConnectionResource planned)
    {
        Log.Logger.Information("U");
        planned.ServicePrincipalId = Guid.NewGuid().ToString();

        return Task.FromResult(planned);

    }
}


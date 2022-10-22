using System.ComponentModel;
using MessagePack;
using TerraformPluginDotNet.Resources;
using TerraformPluginDotNet.Serialization;

namespace TerraformAzurePlugin;

[SchemaVersion(1)]
[MessagePackObject]
public class ServiceConnectionResource
{
    [Key("id")]
    [Description("Combined ID of ServiceConnectionId and ProjectId")]
    [Computed]
    [MessagePackFormatter(typeof(ComputedStringValueFormatter))]
    public string? Id { get; set; } = null!;

    [Key("service_connection_id")]
    [Description("ID of the project in Azure DevOps")]
    [Required]
    [MessagePackFormatter(typeof(ComputedStringValueFormatter))]
    public string? ServiceConnectionId { get; set; } = null!;

    [Key("project_id")]
    [Description("ID of the project in Azure DevOps")]
    [Required]
    [MessagePackFormatter(typeof(ComputedStringValueFormatter))]
    public string? ProjectId { get; set; } = null!;

    [Key("service_principal_application_id")]
    [Description("ID of the service principal assigned to the service connection")]
    [Computed]
    [MessagePackFormatter(typeof(ComputedStringValueFormatter))]
    public string? ServicePrincipalApplicationId { get; set; } = null!;
}

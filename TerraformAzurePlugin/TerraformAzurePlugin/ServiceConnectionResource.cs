using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MessagePack;
using TerraformPluginDotNet.Resources;
using TerraformPluginDotNet.Serialization;
using Key = MessagePack.KeyAttribute;

namespace TerraformAzurePlugin;

[SchemaVersion(1)]
[MessagePackObject]
public class ServiceConnectionResource
{
    [Key("id")]
    [Description("ID of the service connection in Azure DevOps")]
    [Required]
    [MessagePackFormatter(typeof(ComputedValueFormatter))]
    public string Id { get; set; } = null!;

    [Key("project_id")]
    [Description("ID of the project in Azure DevOps")]
    [Required]
    [MessagePackFormatter(typeof(ComputedValueFormatter))]
    public string ProjectId { get; set; } = null!;

    [Key("service_principal_id")]
    [Description("ID of the service principal assigned to the service connection")]
    [Computed]
    [MessagePackFormatter(typeof(ComputedValueFormatter))]
    public string? ServicePrincipalId { get; set; } = null!;
}

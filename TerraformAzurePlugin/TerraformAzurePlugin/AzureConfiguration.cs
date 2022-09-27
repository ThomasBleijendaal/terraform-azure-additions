using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MessagePack;
using TerraformPluginDotNet.Resources;
using Key = MessagePack.KeyAttribute;

namespace TerraformAzurePlugin;

[SchemaVersion(1)]
[MessagePackObject]
public class AzureConfiguration
{
    [Key("org_service_url")]
    [Description("Service url of the DevOps instance")]
    public string ServiceUrl { get; set; } = null!;

    [Key("personal_access_token")]
    [Description("Personal access token")]
    public string PersonalAccessToken { get; set; } = null!;
}


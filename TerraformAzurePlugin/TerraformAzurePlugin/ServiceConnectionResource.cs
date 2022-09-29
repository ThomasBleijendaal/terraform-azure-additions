using System.ComponentModel;
using System.Text;
using MessagePack;
using MessagePack.Formatters;
using TerraformPluginDotNet.Resources;

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


public class ComputedStringValueFormatter : IMessagePackFormatter<string>, IMessagePackFormatter
{
    public string Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        if (reader.NextMessagePackType == MessagePackType.Extension && 
            reader.TryReadExtensionFormatHeader(out var extensionHeader) && extensionHeader.TypeCode == 0)
        {
            reader.Skip();
            return null;
        }

        return reader.ReadString();
    }

    public void Serialize(ref MessagePackWriter writer, string value, MessagePackSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteExtensionFormat(new ExtensionResult(0, new byte[1]));
        }
        else
        {
            writer.WriteString(Encoding.UTF8.GetBytes(value));
        }
    }
}

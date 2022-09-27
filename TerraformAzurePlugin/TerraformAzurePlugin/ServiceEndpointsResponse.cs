using System.Text.Json.Serialization;

namespace TerraformAzurePlugin;

public class ServiceEndpointsResponse
{
    [JsonPropertyName("value")]
    public ServiceEndpoint[]? Value { get; set; }

    public class ServiceEndpoint
    {
        [JsonPropertyName("authorization")]
        public Authorization? Authorization { get; set; }
    }

    public class Authorization
    {
        [JsonPropertyName("parameters")]
        public Parameters? Parameters { get; set; }
    }

    public class Parameters
    {
        [JsonPropertyName("serviceprincipalid")]
        public string? ServicePrincipalId { get; set; }
    }
}

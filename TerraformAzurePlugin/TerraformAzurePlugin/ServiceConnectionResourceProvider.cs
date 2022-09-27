using System.Net.Http.Headers;
using System.Text;
using TerraformPluginDotNet.ResourceProvider;

namespace TerraformAzurePlugin;

public class ServiceConnectionResourceProvider : IResourceProvider<ServiceConnectionResource>
{
    private readonly AzureConfigurator _azureConfigurator;
    private readonly HttpClient _httpClient;

    public ServiceConnectionResourceProvider(
        AzureConfigurator azureConfigurator,
        HttpClient httpClient)
    {
        _azureConfigurator = azureConfigurator;
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(Encoding.UTF8.GetBytes($":{_azureConfigurator.Config.PersonalAccessToken}")));
    }

    public async Task<ServiceConnectionResource> CreateAsync(ServiceConnectionResource planned) => await CreateNewResourceAsync(planned);

    public Task DeleteAsync(ServiceConnectionResource resource) => Task.CompletedTask;

    public Task<ServiceConnectionResource> PlanAsync(ServiceConnectionResource? prior, ServiceConnectionResource proposed)
    {
        var resource = new ServiceConnectionResource
        {
            Id = proposed.Id,
            ProjectId = proposed.ProjectId
        };

        if (prior?.Id != proposed.Id)
        {
            resource.ServicePrincipalId = null;
        }

        return Task.FromResult(resource);
    }

    public async Task<ServiceConnectionResource> ReadAsync(ServiceConnectionResource resource) => await CreateNewResourceAsync(resource);

    public async Task<ServiceConnectionResource> UpdateAsync(ServiceConnectionResource? prior, ServiceConnectionResource planned) => await CreateNewResourceAsync(planned);
    
    private async Task<ServiceConnectionResource> CreateNewResourceAsync(ServiceConnectionResource resource)
        => new ServiceConnectionResource
        {
            Id = resource.Id,
            ProjectId = resource.ProjectId,
            ServicePrincipalId = await GetServicePrincipalIdOfServiceConnectionAsync(resource)
        };

    private async Task<string?> GetServicePrincipalIdOfServiceConnectionAsync(ServiceConnectionResource resource)
    {
        if (string.IsNullOrEmpty(resource.Id) || string.IsNullOrEmpty(resource.ProjectId))
        {
            return null;
        }

        try
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceEndpointsResponse>(
                $"{_azureConfigurator.Config.ServiceUrl}/{resource.ProjectId}/_apis/serviceendpoint/endpoints?endpointIds={resource.Id}&api-version=7.1-preview.4");

            return response?.Value?.FirstOrDefault()?.Authorization?.Parameters?.ServicePrincipalId;
        }
        catch
        {
            return null;
        }
    }
}

using System.Net.Http.Headers;
using System.Text;
using TerraformPluginDotNet.ResourceProvider;

namespace TerraformAzurePlugin;

public class ServiceConnectionResourceProvider : IDataSourceProvider<ServiceConnectionResource>
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

    public async Task<ServiceConnectionResource> ReadAsync(ServiceConnectionResource resource) => await CreateNewResourceAsync(resource);

    private async Task<ServiceConnectionResource> CreateNewResourceAsync(ServiceConnectionResource resource)
    {
        ServiceConnectionResource newResource;

        if (string.IsNullOrEmpty(resource.ProjectId) || string.IsNullOrEmpty(resource.ServiceConnectionId))
        {
            if (string.IsNullOrEmpty(resource.Id))
            {
                newResource = new ServiceConnectionResource();
            }
            else
            {
                var ids = resource.Id.Split("/");

                newResource = new ServiceConnectionResource
                {
                    Id = resource.Id,
                    ProjectId = ids[0],
                    ServiceConnectionId = ids[1]
                };
            }
        }
        else
        {
            newResource = resource;
        }

        newResource.Id = string.IsNullOrEmpty(newResource.ProjectId) || string.IsNullOrEmpty(newResource.ServiceConnectionId)
            ? null : $"{newResource.ProjectId}/{newResource.ServiceConnectionId}";
        newResource.ServicePrincipalApplicationId ??= await GetServicePrincipalIdOfServiceConnectionAsync(newResource);

        return newResource;
    }

    private async Task<string?> GetServicePrincipalIdOfServiceConnectionAsync(ServiceConnectionResource resource)
    {
        if (string.IsNullOrEmpty(resource.ServiceConnectionId) || string.IsNullOrEmpty(resource.ProjectId))
        {
            return null;
        }

        try
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceEndpointsResponse>(
                $"{_azureConfigurator.Config.ServiceUrl}/{resource.ProjectId}/_apis/serviceendpoint/endpoints?endpointIds={resource.ServiceConnectionId}&api-version=7.1-preview.4");

            return response?.Value?.FirstOrDefault()?.Authorization?.Parameters?.ServicePrincipalId;
        }
        catch
        {
            return null;
        }
    }
}

using System.Net.Http.Json;
using CheckAvailability.Client.Models;
using CheckAvailability.Shared;
using Radzen;

namespace CheckAvailability.Client.Services;

public interface IEmployeeService
{
    Task<List<EmployeeModel>> GetEmployees(CancellationToken cancellationToken);
    Task<AvailableTimeResponse> FindAvailableTime(FindAvailableTimeRequest request, CancellationToken cancellationToken);
}

public class EmployeeService : IEmployeeService
{
    private readonly HttpClient _httpClient;

    public EmployeeService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(App.HttpClientName);
    }

    public async Task<List<EmployeeModel>> GetEmployees(CancellationToken cancellationToken)
    {
        return await _httpClient.GetFromJsonAsync<List<EmployeeModel>>("employee", cancellationToken);
    }

    public async Task<AvailableTimeResponse> FindAvailableTime(FindAvailableTimeRequest request, CancellationToken cancellationToken)
    {
       var response = await _httpClient.PostAsJsonAsync("employee/find_available", request, cancellationToken);
       if (!response.IsSuccessStatusCode) return null;
       return await response.ReadAsync<AvailableTimeResponse>();
    }
}
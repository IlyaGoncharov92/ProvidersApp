using Api.Models;
using AutoMapper;
using RestSharp;

namespace Api.Services;

public class ProvidersHttpHttpService(IMapper _mapper) : IProvidersHttpService
{
    // TODO: в реальном проекте я бы вынес это в appSettings.json
    private const string _searchResource = "search";

    public async Task<Route[]> GetRoutesFromProviderOne(SearchRequest searchRequest, CancellationToken cancellationToken)
    {
        var response = await GetRoutesFromProvider<ProviderOneSearchRequest, ProviderOneSearchResponse>(
            ProvidersData.UrlProviderOne,
            searchRequest,
            cancellationToken
        );
        return response == null ? Array.Empty<Route>() : _mapper.Map<Route[]>(response.Routes);
    }

    public async Task<Route[]> GetRoutesFromProviderTwo(SearchRequest searchRequest, CancellationToken cancellationToken)
    {
        var response = await GetRoutesFromProvider<ProviderTwoSearchRequest, ProviderTwoSearchResponse>(
            ProvidersData.UrlProviderTwo,
            searchRequest,
            cancellationToken
        );
        return response == null ? Array.Empty<Route>() : _mapper.Map<Route[]>(response.Routes);
    }

    public async Task<bool> CheckProviderAvailability(string url, CancellationToken cancellationToken)
    {
        var client = new RestClient();
        var request = new RestRequest(url, Method.Get);
        var response = await client.ExecuteAsync(request, cancellationToken);
        return response.IsSuccessful;
    }

    private async Task<TResponse?> GetRoutesFromProvider<TRequest, TResponse>(
        string providerBaseUrl,
        SearchRequest searchRequest, 
        CancellationToken cancellationToken
    )
        where TRequest : class
    {
        var client = new RestClient(providerBaseUrl);
        var request = new RestRequest(_searchResource, Method.Post);

        var providerRequest = _mapper.Map<TRequest>(searchRequest);

        request.AddJsonBody(providerRequest);
        var response = await client.ExecuteAsync<TResponse>(request, cancellationToken);
        return response.Data;
    }
}
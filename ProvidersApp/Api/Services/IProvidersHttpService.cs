namespace Api.Services;

public interface IProvidersHttpService
{
    public Task<Route[]> GetRoutesFromProviderOne(SearchRequest searchRequest, CancellationToken cancellationToken);
    public Task<Route[]> GetRoutesFromProviderTwo(SearchRequest searchRequest, CancellationToken cancellationToken);
    public Task<bool> CheckProviderAvailability(string url, CancellationToken cancellationToken);
}
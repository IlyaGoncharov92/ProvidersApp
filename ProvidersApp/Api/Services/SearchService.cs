namespace Api.Services;

public class SearchService(IProvidersService _providersService) : ISearchService
{
    // TODO: в реальном проекте я бы вынес это в appSettings.json
    private const string _searchResource = "ping";
    
    public async Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken)
    {
        if (request.Filters?.OnlyCached == true)
        {
            return await _providersService.SearchFromCache(request, cancellationToken);
        }
        else
        {
            return await _providersService.SearchFromHttp(request, cancellationToken);
        }
    }
    
    // TODO: Сейчас возвращает true, только если доступны все провайдеры
    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
    {
        var urls = new[]
        {
            $"{ProvidersData.UrlProviderOne}/{_searchResource}", 
            $"{ProvidersData.UrlProviderTwo}/{_searchResource}"
        };
        var results = await _providersService.IsAvailable(urls, cancellationToken);
        return results.All(r => r);
    }
}

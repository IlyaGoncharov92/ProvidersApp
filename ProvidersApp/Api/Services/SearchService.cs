namespace Api.Services;

public class SearchService(IProvidersService _providersService) : ISearchService
{
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
        var results = await _providersService.IsAvailable(cancellationToken);
        return results.All(r => r);
    }
}

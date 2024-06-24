namespace Api.Services;

public interface IProvidersService
{
    public Task<SearchResponse> SearchFromCache(SearchRequest request, CancellationToken cancellationToken);
    public Task<SearchResponse> SearchFromHttp(SearchRequest request, CancellationToken cancellationToken);
    public Task<bool[]> IsAvailable(CancellationToken cancellationToken);
}
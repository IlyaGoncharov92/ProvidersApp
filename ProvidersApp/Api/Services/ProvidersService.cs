using Microsoft.Extensions.Caching.Memory;

namespace Api.Services;

public class ProvidersService(IProvidersHttpService _providersHttpService, IMemoryCache _cache) : IProvidersService
{
    private const string _cacheKey = "Routes";

    // TODO: в реальном проекте я бы вынес это в appSettings.json
    private readonly TimeSpan _cacheExp = TimeSpan.FromMinutes(30);
    private const string _searchResource = "ping";

    // TODO: по идее, если время кэша истекло, то нужно сделать еще один http запрос
    public async Task<SearchResponse> SearchFromCache(SearchRequest request, CancellationToken cancellationToken)
    {
        var routes = _cache.TryGetValue(_cacheKey, out List<Route> cachedRoutes)
            ? cachedRoutes
            : [];

        return FilterRoutes(routes, request.Filters);
    }

    // TODO: сейчас реализовано полное обновление кэша при каждом запросе
    public async Task<SearchResponse> SearchFromHttp(SearchRequest request, CancellationToken cancellationToken)
    {
        var tasks = new List<Task<Route[]>>
        {
            _providersHttpService.GetRoutesFromProviderOne(request, cancellationToken),
            _providersHttpService.GetRoutesFromProviderTwo(request, cancellationToken)
        };

        var routes = (await Task.WhenAll(tasks)).SelectMany(r => r).ToList();

        _cache.Set(_cacheKey, routes, _cacheExp);

        return FilterRoutes(routes, request.Filters);
    }

    private SearchResponse FilterRoutes(List<Route> routes, SearchFilters filters)
    {
        if (filters != null)
        {
            routes = routes.Where(r =>
                (!filters.DestinationDateTime.HasValue || r.DestinationDateTime <= filters.DestinationDateTime.Value) &&
                (!filters.MaxPrice.HasValue || r.Price <= filters.MaxPrice.Value) &&
                (!filters.MinTimeLimit.HasValue || r.TimeLimit >= filters.MinTimeLimit.Value)
            ).ToList();
        }

        return new SearchResponse
        {
            Routes = routes.ToArray(),
            MinPrice = routes.Min(r => r.Price),
            MaxPrice = routes.Max(r => r.Price),
            MinMinutesRoute = routes.Min(r => (int) (r.DestinationDateTime - r.OriginDateTime).TotalMinutes),
            MaxMinutesRoute = routes.Max(r => (int) (r.DestinationDateTime - r.OriginDateTime).TotalMinutes)
        };
    }

    public async Task<bool[]> IsAvailable(CancellationToken cancellationToken)
    {
        var urls = new[]
        {
            $"{ProvidersData.UrlProviderOne}/{_searchResource}", 
            $"{ProvidersData.UrlProviderTwo}/{_searchResource}"
        };
        var tasks = urls.Select(url => _providersHttpService.CheckProviderAvailability(url, cancellationToken));
        return await Task.WhenAll(tasks);
    }
}
using Api;
using Api.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddAutoMapper(typeof(AutoMapperConfig));

services.AddSingleton<ISearchService, SearchService>();
services.AddSingleton<IProvidersService, ProvidersService>();
services.AddSingleton<IProvidersHttpService, ProvidersHttpHttpService>();

services.AddControllers();

services.AddMemoryCache();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
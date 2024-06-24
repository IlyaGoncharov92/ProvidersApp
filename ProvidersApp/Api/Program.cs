using Api;
using Api.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddSingleton<ISearchService, SearchService>();
services.AddSingleton<IProvidersService, ProvidersService>();
services.AddSingleton<IProvidersHttpService, ProvidersHttpHttpService>();

services.AddControllers();

services.AddAutoMapper(typeof(AutoMapperConfig));

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
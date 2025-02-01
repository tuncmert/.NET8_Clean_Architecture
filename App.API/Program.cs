using App.API.Filters;
using App.Persistence.Extensions;
using App.Application.Extensions;
using App.API.ExceptionHandler;
using App.Application.Contracts.Caching;
using App.Caching;
using App.API.Extensions;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithFiltersExt().AddSwaggerGenExt().AddExceptionHandlerExt().AddCachingExt();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddRepositories(builder.Configuration).AddServices(builder.Configuration);


var app = builder.Build();
app.UseConfigurePipelineExt();

app.MapControllers();

app.Run();

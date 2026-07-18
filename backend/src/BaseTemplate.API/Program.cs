using Asp.Versioning.ApiExplorer;
using BaseTemplate.API.Extensions;
using BaseTemplate.API.Middleware;
using BaseTemplate.Application;
using BaseTemplate.Infrastructure;
using BaseTemplate.Infrastructure.Persistence;
using BaseTemplate.Infrastructure.Persistence.Seed;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services));

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApiVersioningSupport();
builder.Services.AddCorsForAngular(builder.Configuration);
builder.Services.AddJwtAuthenticationScaffolding(builder.Configuration);

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();
        foreach (var description in descriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
    });

    using var scope = app.Services.CreateScope();
    var serviceProvider = scope.ServiceProvider;
    var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
    await ApplicationDbContextSeed.SeedAsync(context, serviceProvider, logger);
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors(ApiServiceCollectionExtensions.CorsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Exposed for WebApplicationFactory-based integration tests.
public partial class Program
{
}

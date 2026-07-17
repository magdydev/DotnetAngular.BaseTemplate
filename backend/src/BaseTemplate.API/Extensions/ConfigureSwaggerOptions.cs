using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BaseTemplate.API.Extensions;

/// <summary>Generates one Swagger document per discovered API version (v1, v2, ...).</summary>
public sealed class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = "BaseTemplate API",
                Version = description.ApiVersion.ToString(),
                Description = description.IsDeprecated ? "This API version has been deprecated." : "DDD Clean Architecture base template API.",
            });
        }

        var jwtScheme = new OpenApiSecurityScheme
        {
            Scheme = "bearer",
            BearerFormat = "JWT",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Description = "Enter a valid JWT bearer token.",
            Reference = new OpenApiReference
            {
                Id = JwtBearerDefaults.AuthenticationScheme,
                Type = ReferenceType.SecurityScheme,
            },
        };

        options.AddSecurityDefinition(jwtScheme.Reference.Id, jwtScheme);
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { jwtScheme, Array.Empty<string>() },
        });
    }
}

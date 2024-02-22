using System.Text.Json.Serialization;
using ControlPlane.API.Extensions;
using ControlPlane.API.Middlewares;
using ControlPlane.API.Services;
using ControlPlane.API.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
// Configuration 
builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDB"));
builder.Services.Configure<ClientCredentialsSettings>(
    builder.Configuration.GetSection("ClientCredentials"));
builder.Services.Configure<MobileNetworkSettings>(
builder.Configuration.GetSection("MobileNetwork"));

// Dependencies
builder.Services.AddSingleton<MongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    var client = new MongoClient(settings.ConnectionString);
    return client;
});

builder.Services.AddSingleton<CameraService>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "api-key",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "ApiKey",
        Description = "Api Key"
    });
});
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new CameraTypeConverter());
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddScoped(sp =>
{
    var settings = sp.GetRequiredService<IOptions<ClientCredentialsSettings>>().Value;
    return new ClientApplicationHttpClientHandler(ConfidentialClientApplicationBuilder
        .Create(settings.ClientId)
        .WithClientSecret(settings.ClientSecret)
        .WithAuthority(new Uri(settings.Authority))
        .Build(),
        ["https://management.azure.com/.default"]);
});

builder.Services.AddAzureHttpClient<SimGroupService>();
builder.Services.AddAzureHttpClient<SimService>();
builder.Services.AddAzureHttpClient<SimPolicyService>();
builder.Services.AddAzureHttpClient<AttachedDataNetworkService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CORS", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ApiKeyMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseCors("CORS");

// app.UseHttpsRedirection();
app.MapControllers();

app.Run();

using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Configuration;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Infraestructure;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Models;
using Microsoft.MecSolutionAccelerator.Services.Alerts.API.Injection;
using Microsoft.MecSolutionAccelerator.Services.Alerts.API.Jobs;
using Coravel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDaprClient();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAlertsRepository, AlertsNoSqlRepository>();

var mongoConfig = builder.Configuration.GetSection("MongoDB").Get<MongoDbConfiguration>();
builder.Services.AddSingleton(config => mongoConfig);

builder.Services.AddBoundingBoxesColorConfiguration(builder.Configuration);

builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<CosmosDbCleanupJob>();
builder.Services.AddScheduler();
builder.Services.AddSingleton<IAlertsRepository, AlertsNoSqlRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Services.UseScheduler(scheduler =>
{
    scheduler.Schedule<CosmosDbCleanupJob>()
    .Weekly()
    .Weekday();
});

app.UseCloudEvents();
app.UseAuthorization();
app.MapControllers();
app.MapSubscribeHandler();
app.Run();

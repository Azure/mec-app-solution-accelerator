using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Configuration;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Injection;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Models;
using RulesEngine.Infraestructure;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDaprClient();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IDetectionsRepository, DetectionsNoSqlRepository>();

var mongoConfig = builder.Configuration.GetSection("MongoDB").Get<MongoDbConfiguration>();
builder.Services.AddSingleton(config => mongoConfig);
builder.Services.AddRulesEngineConfiguration(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCloudEvents();
app.UseAuthorization();
app.MapControllers();
app.MapSubscribeHandler();
app.Run();

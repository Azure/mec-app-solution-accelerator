using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Infraestructure;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDaprClient();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAlertsRepository, AlertsRepository>();
var dbServer = builder.Configuration["DBServer"] ?? "sqlserver";
var dbPort = builder.Configuration["DBPort"] ?? "1433";
var dbUser = builder.Configuration["DBUser"] ?? "SA";
var password = builder.Configuration["DBPassword"];
var database = builder.Configuration["Database"] ?? "Alerts";

builder.Services.AddDbContext<AlertsDbContext>(options =>
       options.UseSqlServer($"Server={dbServer}, {dbPort};Initial Catalog={database}; User ID={dbUser}; Password={password}; MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False;"));

builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AlertsDbContext>();
    var created = db.Database.EnsureCreated();
}

app.UseCloudEvents();
app.UseAuthorization();
app.MapControllers();
app.MapSubscribeHandler();
app.Run();

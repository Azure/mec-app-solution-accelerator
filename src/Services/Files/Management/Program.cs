using Coravel;
using Microsoft.MecSolutionAccelerator.Services.Files.Infraestructure;
using Microsoft.MecSolutionAccelerator.Services.FilesManagement.Jobs;
using Microsoft.MecSolutionAccelerator.Services.MinIOInfraestructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IFileStorage, MinIOService>();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
builder.Services.AddScoped<MinioCleanUpJob>();
builder.Services.AddScheduler();

builder.Services.Configure<MinioConfiguration>(builder.Configuration.GetSection("Minio"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection(); // Apply HTTPS redirection only in non-development environments
}
app.Services.UseScheduler(scheduler =>
{
    scheduler.Schedule<MinioCleanUpJob>()
    .EveryThirtyMinutes().RunOnceAtStart();
});
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

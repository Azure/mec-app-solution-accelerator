using FilesManagement.CommandHandler;
using FilesManagement.Configuration;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(typeof(UploadNewFileCommandHandler).Assembly);
builder.Services.AddMediatR(typeof(DownloadFileCommandHandler).Assembly);
builder.Logging.AddConsole();
builder.Logging.AddDebug();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

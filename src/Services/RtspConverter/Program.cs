using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using RtspConverter.Models;
using RtspConverter.Services;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;
                var mongoDbConfiguration = configuration.GetSection("MongoDB"); ;
                var connectionString = mongoDbConfiguration.GetValue<string>("ConnectionString");
                var databaseName = mongoDbConfiguration.GetValue<string>("DatabaseName");
                var mongoClient = new MongoClient(connectionString);
                var database = mongoClient.GetDatabase(databaseName);
                var cameraCollection = database.GetCollection<BsonDocument>("Cameras");

                services.AddSingleton<SharedCameraState>();
                services.AddSingleton<IMongoCollection<BsonDocument>>(cameraCollection);
                services.AddHostedService<CameraTrackingService>();
                services.AddHostedService<WatchdogService>();
            });
}
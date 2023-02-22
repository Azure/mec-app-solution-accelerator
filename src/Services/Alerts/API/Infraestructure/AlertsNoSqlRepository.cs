using Microsoft.MecSolutionAccelerator.Services.Alerts.Configuration;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Models;
using MongoDB.Driver;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.Infraestructure
{
    public class AlertsNoSqlRepository : IAlertsRepository
    {
        private readonly MongoDbConfiguration _config;
        private readonly IMongoDatabase _database;
        private readonly IMongoClient _dbClient;

        public AlertsNoSqlRepository(MongoDbConfiguration config)
        {
            var mongoUrlBuilder = new MongoUrlBuilder($"{config.UrlPrefix}://{config.Hostname}:{config.PortNumber}/{config.DatabaseName}");
            _dbClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());
            _database = _dbClient.GetDatabase(config.DatabaseName);
        }

        public async Task Create(Alert entity)
        {
            var collection = _database.GetCollection<Alert>(nameof(Alert));
            await collection.InsertOneAsync(entity);
        }

        public async Task Update(Alert entity)
        {
            var collection = _database.GetCollection<Alert>(nameof(Alert));
            var updateFilter = Builders<Alert>.Filter.Eq("Id", entity.Id);
            await collection.ReplaceOneAsync(updateFilter, entity);
        }

        public async Task Delete(Guid id)
        {
            var collection = _database.GetCollection<Alert>(nameof(Alert));
            var deleteFilter = Builders<Alert>.Filter.Eq("Id", id);
            await collection.DeleteOneAsync(deleteFilter);
        }

        public async Task<Alert> GetById(Guid id)
        {
            var collection = _database.GetCollection<Alert>(nameof(Alert));
            return await collection.Find(Builders<Alert>.Filter.Eq("Id", id)).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Alert>> List()
        {
            var collection = _database.GetCollection<Alert>(nameof(Alert));
            return await collection.AsQueryable().ToListAsync();
        }

        public IEnumerable<Alert> List(int skip, int take)
        {
            var collection = _database.GetCollection<Alert>(nameof(Alert));
            return collection.AsQueryable().OrderByDescending(alert => alert.CaptureTime).Skip(skip).Take(take).ToList();
        }

        public async Task<IEnumerable<AlertMinimized>> GetAlertsMinimized(int skip, int take)
        {
            var collection = _database.GetCollection<Alert>(nameof(Alert));

            var result = await collection
                .Find(alert => true)
                .Project<AlertMinimized>(Builders<Alert>.Projection
                .Include("Information")
                .Include("CaptureTime")
                .Include("AlertTime")
                .Include("MsExecutionTime")
                .Include("Type")
                .Include("Accuracy")
                .Include("Source"))
                .SortByDescending(alert => alert.CaptureTime)
                .Skip(skip)
                .Limit(take)
                .ToListAsync();

            return result;
        }
    }
}

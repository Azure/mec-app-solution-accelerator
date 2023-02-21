using Microsoft.MecSolutionAccelerator.Services.Alerts.Configuration;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Models;
using MongoDB.Bson;
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

        private IMongoDatabase GetDatabase()
        {
            return _database;
        }

        public async Task Create(Alert entity)
        {
            var collection = this.GetDatabase().GetCollection<Alert>(typeof(Alert).Name);
            await collection.InsertOneAsync(entity);
        }

        public async Task Delete(Guid id)
        {
            var collection = this.GetDatabase().GetCollection<Alert>(typeof(Alert).Name);
            var deleteFilter = Builders<Alert>.Filter.Eq("Id", id);
            await collection.DeleteOneAsync(deleteFilter);
        }

        public async Task<Alert> GetById(Guid id)
        {
            var collection = this.GetDatabase().GetCollection<Alert>(typeof(Alert).Name);
            return await (await collection.FindAsync(alert => alert.Id.Equals(id))).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Alert>> List()
        {
            var collection = this.GetDatabase().GetCollection<Alert>(typeof(Alert).Name);
            return await(await collection.FindAsync(_ => true)).ToListAsync();
        }

        public IEnumerable<Alert> List(int skip, int take)
        {
            var collection = this.GetDatabase().GetCollection<Alert>(typeof(Alert).Name);
            return collection.Find(_ => true).SortByDescending(bson => bson.CaptureTime).Skip(skip).Limit(take).ToList();
        }

        public async Task Update(Alert entity)
        {
            var collection = this.GetDatabase().GetCollection<Alert>(typeof(Alert).Name);
            var updateFilter = Builders<Alert>.Filter.Eq("Id", entity.Id);
            await collection.ReplaceOneAsync(updateFilter, entity, new ReplaceOptions() { IsUpsert = false });
        }

        public async Task<IEnumerable<AlertMinimized>> GetAlertsMinimized(int skip, int take)
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$project", new BsonDocument
                {
                    { "_id", 1 },
                    { "Information", 1 },
                    { "CaptureTime", 1 },
                    { "AlertTime", 1 },
                    { "MsExecutionTime", 1 },
                    { "Type", 1 },
                    { "Accuracy", 1 },
                    { "Source", 1 }
                }),
                new BsonDocument("$sort", new BsonDocument
                {
                    { "CaptureTime", -1 }
                }),
                new BsonDocument("$skip", skip),
                new BsonDocument("$limit", take)
            };

            var collection = this.GetDatabase().GetCollection<Alert>(typeof(Alert).Name);

            var options = new AggregateOptions { AllowDiskUse = true };
            var result = await collection.Aggregate<AlertMinimized>(pipeline, options).ToListAsync();

            return result;
        }
    }
}

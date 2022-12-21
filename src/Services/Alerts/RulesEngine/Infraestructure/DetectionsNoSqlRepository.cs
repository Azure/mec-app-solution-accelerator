using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Configuration;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Models;
using MongoDB.Driver;

namespace RulesEngine.Infraestructure
{
    public class DetectionsNoSqlRepository : IDetectionsRepository
    {
        private readonly MongoDbConfiguration _config;
        public DetectionsNoSqlRepository(MongoDbConfiguration config)
        {
            this._config = config;
        }

        private IMongoDatabase GetDatabase()
        {
            var mongoUrlBuilder = new MongoUrlBuilder($"{this._config.UrlPrefix}://{this._config.Hostname}:{this._config.PortNumber}/{this._config.DatabaseName}");
            MongoClient dbClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());
            var database = dbClient.GetDatabase(this._config.DatabaseName);

            return database;
        }

        private IMongoDatabase GetDatabase(string databaseName)
        {
            var mongoUrlBuilder = new MongoUrlBuilder($"{this._config.UrlPrefix}://{this._config.Hostname}:{this._config.PortNumber}/{databaseName}");
            MongoClient dbClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());
            var database = dbClient.GetDatabase(databaseName);

            return database;
        }

        public async Task CreateInCollection(Detection entity)
        {
            var collection = this.GetDatabase(entity.EventType).GetCollection<Detection>(typeof(Detection).Name);
            await collection.InsertOneAsync(entity);
        }

        public async Task Create(Detection entity)
        {

            var collection = this.GetDatabase().GetCollection<Detection>(typeof(Detection).Name);
            await collection.InsertOneAsync(entity);
        }

        public async Task Delete(Guid id)
        {
            var collection = this.GetDatabase().GetCollection<Detection>(typeof(Detection).Name);
            var deleteFilter = Builders<Detection>.Filter.Eq("Id", id);
            await collection.DeleteOneAsync(deleteFilter);
        }

        public async Task<Detection> GetById(Guid id)
        {
            var collection = this.GetDatabase().GetCollection<Detection>(typeof(Detection).Name);
            return await(await collection.FindAsync(alert => alert.Id.Equals(id))).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Detection>> List()
        {
            var collection = this.GetDatabase().GetCollection<Detection>(typeof(Detection).Name);
            return await(await collection.FindAsync(_ => true)).ToListAsync();
        }

        public IEnumerable<Detection> List(int skip, int take)
        {
            var collection = this.GetDatabase().GetCollection<Detection>(typeof(Detection).Name);
            return collection.Find(_ => true).Skip(skip).Limit(take).ToList();
        }

        public async Task Update(Detection entity)
        {
            var collection = this.GetDatabase().GetCollection<Detection>(typeof(Detection).Name);
            var updateFilter = Builders<Detection>.Filter.Eq("Id", entity.Id);
            await collection.ReplaceOneAsync(updateFilter, entity, new ReplaceOptions() { IsUpsert = false });
        }

        public async Task<List<Detection>> GetFramesByClassNextInTime(long time, string @class)
        {
            var collection = this.GetDatabase(@class).GetCollection<Detection>(typeof(Detection).Name);
            return collection.Find(x => x.EveryTime -time == 2 || x.EveryTime - time == -2 ).ToList();
        }
    }
}

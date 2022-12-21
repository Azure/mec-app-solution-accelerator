using Microsoft.MecSolutionAccelerator.Services.Alerts.Configuration;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Models;
using MongoDB.Driver;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.Infraestructure
{
    public class AlertsNoSqlRepository : IAlertsRepository
    {
        private readonly MongoDbConfiguration _config;
        public AlertsNoSqlRepository(MongoDbConfiguration config)
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
            var today = DateTime.Now;
            var yesterday = today.AddDays(-1);
            return collection.Find(x => x.AlertTriggerTimeFin > yesterday).Skip(skip).Limit(take).ToList();
        }

        public async Task Update(Alert entity)
        {
            var collection = this.GetDatabase().GetCollection<Alert>(typeof(Alert).Name);
            var updateFilter = Builders<Alert>.Filter.Eq("Id", entity.Id);
            await collection.ReplaceOneAsync(updateFilter, entity, new ReplaceOptions() { IsUpsert = false });
        }
    }
}

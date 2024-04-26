using MongoDB.Driver;
using MongoDB.Bson;
using System;
using CheesyCroco.Models.Collections;
using CheesyCroco.Settings;
using Microsoft.Extensions.Options;

namespace CheesyCroco.Services
{
    public class ResultService : IService<Result>
    {
        private IMongoCollection<Result> _results;

        public ResultService(IOptions<MongoDBSettings> mongoDbSettings)
        {
            var client = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _results = database.GetCollection<Result>("Results");
        }

        public List<Result> GetAll()
        {
            return _results.Find(FilterDefinition<Result>.Empty).ToList();
        }

        public Result GetById(string id)
        {
            return _results.Find(x => x.Id == id).FirstOrDefault();
        }

        public void SaveOrUpdate(Result obj)
        {
            var resultObj = _results.Find(x => x.Id == obj.Id).FirstOrDefault();

            if (resultObj == null)
            {
                _results.InsertOne(obj);
            }
            else
            {
                _results.ReplaceOne(x => x.Id == obj.Id, obj);
            }
        }
    }
}

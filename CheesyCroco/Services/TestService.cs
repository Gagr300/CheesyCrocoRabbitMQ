using MongoDB.Driver;
using MongoDB.Bson;
using System;
using CheesyCroco.Models.Collections;
using CheesyCroco.Settings;
using Microsoft.Extensions.Options;

namespace CheesyCroco.Services
{
    public class TestService : IService<Test>
    {
        private IMongoCollection<Test> _tests;

        public TestService(IOptions<MongoDBSettings> mongoDbSettings)
        {
            var client = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _tests = database.GetCollection<Test>("Tests");
        }

        public List<Test> GetAll()
        {
            return _tests.Find(FilterDefinition<Test>.Empty).ToList();
        }

        public Test GetById(string id)
        {
            return _tests.Find(x => x.Id == id).FirstOrDefault();
        }

        public void SaveOrUpdate(Test obj)
        {
            var testObj = _tests.Find(x => x.Id == obj.Id).FirstOrDefault();

            if (testObj == null)
            {
                _tests.InsertOne(obj);
            }
            else
            {
                _tests.ReplaceOne(x => x.Id == obj.Id, obj);
            }
        }


    }
}

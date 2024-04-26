using MongoDB.Driver;
using MongoDB.Bson;
using System;
using CheesyCroco.Models.Collections;
using CheesyCroco.Settings;
using Microsoft.Extensions.Options;

namespace CheesyCroco.Services
{
    public class QuestionService : IService<Question>
    {
        private IMongoCollection<Question> _questions;

        public QuestionService(IOptions<MongoDBSettings> mongoDbSettings)
        {
            var client = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _questions = database.GetCollection<Question>("Questions");
        }

        public List<Question> GetAll()
        {
            return _questions.Find(FilterDefinition<Question>.Empty).ToList();
        }

        public Question GetById(string id)
        {
            return _questions.Find(x => x.Id == id).FirstOrDefault();
        }

        public void SaveOrUpdate(Question obj)
        {
            var questionObj = _questions.Find(x => x.Id == obj.Id).FirstOrDefault();

            if (questionObj == null)
            {
                _questions.InsertOne(obj);
            }
            else
            {
                _questions.ReplaceOne(x => x.Id == obj.Id, obj);
            }
        }
    }
}

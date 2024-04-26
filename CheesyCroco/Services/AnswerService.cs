using MongoDB.Driver;
using MongoDB.Bson;
using System;
using CheesyCroco.Models.Collections;
using CheesyCroco.Settings;
using Microsoft.Extensions.Options;

namespace CheesyCroco.Services
{
    public class AnswerService : IService<Answer>
    {

        private IMongoCollection<Answer> _answers;

        public AnswerService(IOptions<MongoDBSettings> mongoDbSettings)
        {
            var client = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _answers = database.GetCollection<Answer>("Answers");
        }

        public List<Answer> GetAll()
        {
            return _answers.Find(FilterDefinition<Answer>.Empty).ToList();
        }

        public Answer GetById(string id)
        {
            return _answers.Find(x => x.Id == id).FirstOrDefault();
        }

        public void SaveOrUpdate(Answer obj)
        {
            var answerObj = _answers.Find(x => x.Id == obj.Id).FirstOrDefault();

            if (answerObj == null)
            {
                _answers.InsertOne(obj);
            }
            else
            {
                _answers.ReplaceOne(x => x.Id == obj.Id, obj);
            }
        }
    }
}

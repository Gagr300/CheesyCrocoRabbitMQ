using Rabbit.Models.Collections;
using MongoDB.Driver;

namespace Rabbit.Services
{
    public class AnswerService : IService<Answer>
    {
        private IMongoCollection<Answer> _answers;

        public AnswerService(IMongoCollection<Answer> answers)
        {
            _answers = answers;
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
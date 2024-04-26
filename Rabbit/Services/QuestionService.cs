using Rabbit.Models.Collections;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit.Services
{
    public class QuestionService : IService<Question>
    {

        private IMongoCollection<Question> _questions;

        public QuestionService(IMongoCollection<Question> questions)
        {
            _questions = questions;
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

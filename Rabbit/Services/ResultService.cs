using Rabbit.Models.Collections;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit.Services
{
    public class ResultService : IService<Result>
    {

        private IMongoCollection<Result> _results;

        public ResultService(IMongoCollection<Result> results)
        {
            _results = results;
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

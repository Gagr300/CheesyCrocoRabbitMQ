using Rabbit.Models.Collections;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit.Services
{
    public class TestService : IService<Test>
    {

        private IMongoCollection<Test> _tests;

        public TestService(IMongoCollection<Test> tests)
        {
            _tests = tests;
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

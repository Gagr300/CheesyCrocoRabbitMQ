using Rabbit.Models.Collections;
using Rabbit.Services;
using Rabbit.Settings;
using MongoDB.Driver;

namespace Rabbit.Managers
{
    public class MongoDBManager
    {
        private IMongoDatabase _database;

        public readonly AnswerService _answerService;
        public readonly QuestionService _questionService;
        public readonly ResultService _resultService;
        public readonly TestService _testService;

        public MongoDBManager(MongoDBSettings mongoDbSettings)
        {
            var client = new MongoClient(mongoDbSettings.ConnectionString);
            var _database = client.GetDatabase(mongoDbSettings.DatabaseName);

            _answerService = new AnswerService(_database.GetCollection<Answer>("Answers"));
            _questionService = new QuestionService(_database.GetCollection<Question>("Questions"));
            _resultService = new ResultService(_database.GetCollection<Result>("Results"));
            _testService = new TestService(_database.GetCollection<Test>("Tests"));
        }
    }
}

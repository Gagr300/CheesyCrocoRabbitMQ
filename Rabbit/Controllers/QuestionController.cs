using Rabbit.Managers;
using Rabbit.Models.Collections;
using Rabbit.Models;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Rabbit.Controllers
{
    public class QuestionController
    {
        private readonly MongoDBManager _mongoDbManager;

        public QuestionController(MongoDBManager mongoDbManager)
        {
            _mongoDbManager = mongoDbManager;
        }

        public Response Execute(Request<Question> request)
        {
            string typeOfRequest = request.TypeOfRequest.Substring(request.TypeOfRequest.IndexOf('.') + 1);
            var reqQuestion = request.Body;

            switch (typeOfRequest)
            {
                case "getAll":
                    var response = _mongoDbManager._questionService.GetAll();

                    var options = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                        WriteIndented = true
                    };

                    var responseString = JsonSerializer.Serialize(response, options);

                    Console.WriteLine(responseString);

                    return new Response("Request Get Question Done", responseString);

                default:
                    Console.WriteLine($" [.] EXCEPTION: UserAccountController.execute: неопознанный typeOfRequest={typeOfRequest}");
                    return new Response("ERROR_PROGRAM", "");
            }
        }

    }
}

using Rabbit.Managers;
using Rabbit.Models.Collections;
using Rabbit.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Rabbit.Controllers
{
    public class TestController
    {
        private readonly MongoDBManager _mongoDbManager;

        public TestController(MongoDBManager mongoDbManager)
        {
            _mongoDbManager = mongoDbManager;
        }

        public Response Execute(Request<Test> request)
        {
            string typeOfRequest = request.TypeOfRequest.Substring(request.TypeOfRequest.IndexOf('.') + 1);
            var reqTest = request.Body;

            switch (typeOfRequest)
            {
                case "getAll":
                    var response = _mongoDbManager._testService.GetAll();

                    var options = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                        WriteIndented = true
                    };

                    var responseString = JsonSerializer.Serialize(response, options);

                    Console.WriteLine(responseString);

                    return new Response("Request Get Test Done", responseString);

                case "saveOrUpdate":
                    _mongoDbManager._testService.SaveOrUpdate(reqTest);

                    Console.WriteLine("Test Saved");

                    return new Response("Request SaveOrUpdate Test Done", "");

                default:
                    Console.WriteLine($" [.] EXCEPTION: UserAccountController.execute: неопознанный typeOfRequest={typeOfRequest}");
                    return new Response("ERROR_PROGRAM", "");
            }
        }
    }
}

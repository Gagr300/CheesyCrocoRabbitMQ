using Rabbit.Managers;
using Rabbit.Models.Collections;
using Rabbit.Models;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Rabbit.Controllers
{
    public class ResultController
    {
        private readonly MongoDBManager _mongoDbManager;

        public ResultController(MongoDBManager mongoDbManager)
        {
            _mongoDbManager = mongoDbManager;
        }

        public Response Execute(Request<Result> request)
        {
            string typeOfRequest = request.TypeOfRequest.Substring(request.TypeOfRequest.IndexOf('.') + 1);
            var reqResult = request.Body;

            switch (typeOfRequest)
            {
                case "getAll":
                    var response = _mongoDbManager._resultService.GetAll();

                    var options = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                        WriteIndented = true
                    };

                    var responseString = JsonSerializer.Serialize(response, options);

                    Console.WriteLine(responseString);

                    return new Response("Request Get Result Done", responseString);

                case "saveOrUpdate":
                    _mongoDbManager._resultService.SaveOrUpdate(reqResult);

                    Console.WriteLine("Result Saved");

                    var opts = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                        WriteIndented = true
                    };

                    return new Response("Request SaveOrUpdate Result Done", "");

                default:
                    Console.WriteLine($" [.] EXCEPTION: UserAccountController.execute: неопознанный typeOfRequest={typeOfRequest}");
                    return new Response("ERROR_PROGRAM", "");
            }
        }
    }
}

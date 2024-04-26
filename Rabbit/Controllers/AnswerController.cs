using Rabbit.Managers;
using System;
using Rabbit.Managers;
using Rabbit.Models;
using Rabbit.Models.Collections;
using System.Text;
using Rabbit.Managers;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Rabbit.Controllers
{
    public class AnswerController
    {
        private readonly MongoDBManager _mongoDbManager;

        public AnswerController(MongoDBManager mongoDbManager)
        {
            _mongoDbManager = mongoDbManager;
        }

        public Response Execute(Request<Answer> request)
        {
            string typeOfRequest = request.TypeOfRequest.Substring(request.TypeOfRequest.IndexOf('.') + 1);
            var reqAnswer = request.Body;

            switch (typeOfRequest)
            {
                case "getAll":
                    var response = _mongoDbManager._answerService.GetAll();

                    var options = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                        WriteIndented = true
                    };

                    var responseString = JsonSerializer.Serialize(response, options);

                    Console.WriteLine(responseString);

                    return new Response("Request Get Answer Done", responseString);

                default:
                    Console.WriteLine($" [.] EXCEPTION: UserAccountController.execute: неопознанный typeOfRequest={typeOfRequest}");
                    return new Response("ERROR_PROGRAM", "");
            }
        }
    }
}
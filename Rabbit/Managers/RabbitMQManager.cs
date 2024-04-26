using System.Collections.Specialized;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Rabbit.Controllers;
using Rabbit.Models;
using Rabbit.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Rabbit.Managers;
using Rabbit.Models.Collections;
using Newtonsoft.Json;

namespace Rabbit.Managers
{
    public class RabbitMQManager
    {
        private const string EXCHANGE_NAME = "rpc_exchange";
        private const string QUEUE_NAME = "rpc_queue";
        private const string ROUTING_KEY = "rpc_routing_key";

        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly AnswerController _answerController;
        private readonly QuestionController _questionController;
        private readonly ResultController _resultController;
        private readonly TestController _testController;

        public RabbitMQManager(RabbitMQSettings rabbitMqSettings, MongoDBManager mongoDbManager)
        {
            // 0. Подключение контроллеров

            _answerController = new AnswerController(mongoDbManager);
            _questionController = new QuestionController(mongoDbManager);
            _resultController = new ResultController(mongoDbManager);
            _testController = new TestController(mongoDbManager);
            
            // 1. Настройки RabbitMQ

            ConnectionFactory factory = new();
            factory.Uri = new Uri(rabbitMqSettings.Uri);
            factory.ClientProvidedName = rabbitMqSettings.ClientProvidedName;

            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Direct);
            channel.QueueDeclare(QUEUE_NAME, false, false, false, null);
            channel.QueueBind(QUEUE_NAME, EXCHANGE_NAME, ROUTING_KEY, null);
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queue: QUEUE_NAME,
                                 autoAck: false,
                                 consumer: consumer);

            // 2. Обработчик запроса

            Console.WriteLine(" [x] Awaiting RPC requests");

            consumer.Received += (model, ea) =>
            {
                Response response = new Response();

                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                props.ReplyTo = "rpc_queue";
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                try
                {
                    var message = Encoding.UTF8.GetString(body);
                    string collection = message.Substring(23, message.IndexOf('.') - 23);

                    switch (collection)
                    {
                        case "answer":
                            var answerRequest = JsonConvert.DeserializeObject<Request<Answer>>(message);
                            response = _answerController.Execute(answerRequest);
                            break;
                        case "question":
                            var questionRequest = JsonConvert.DeserializeObject<Request<Question>>(message);
                            response = _questionController.Execute(questionRequest);
                            break;

                        case "result":
                            var resultRequest = JsonConvert.DeserializeObject<Request<Result>>(message);
                            response = _resultController.Execute(resultRequest);
                            break;

                        case "test":
                            var testRequest = JsonConvert.DeserializeObject<Request<Test>>(message);
                            response = _testController.Execute(testRequest);
                            break;
                        
                        default:
                            Console.WriteLine($" [.] EXCEPTION: RabbitMqManager: неопознанная collection={collection}");
                            response = new Response("ERROR_PROGRAM", "");
                            break;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine($" [.] {e.Message}");
                    response = new Response("ERROR_PROGRAM", "");
                }
                finally
                {
                    var options = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                        WriteIndented = true
                    };

                    var responseString = System.Text.Json.JsonSerializer.Serialize(response, options);
                    byte[] responseBytes = Encoding.UTF8.GetBytes(responseString);

                    channel.BasicPublish(exchange: string.Empty,
                                         routingKey: props.ReplyTo,
                                         basicProperties: replyProps,
                                         body: responseBytes);

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();

        }
    }
}
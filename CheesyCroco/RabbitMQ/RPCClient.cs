using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Collections.Concurrent;
using System.Text;

namespace CheesyCroco.RabbitMQ
{
    public class RPCClient
    {
        private const string EXCHANGE_NAME = "rpc_exchange";
        private const string QUEUE_NAME = "rpc_queue";
        private const string ROUTING_KEY = "rpc_routing_key";

        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> callbackMapper = new();

        public RPCClient()
        {
            ConnectionFactory factory = new();
            factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
            factory.ClientProvidedName = "Rabbit Sender App";

            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            // declare a server-named queue

            channel.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Direct);
            channel.QueueDeclare(QUEUE_NAME, false, false, false, null);
            channel.QueueBind(QUEUE_NAME, EXCHANGE_NAME, ROUTING_KEY, null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                if (!callbackMapper.TryRemove(ea.BasicProperties.CorrelationId, out var tcs))
                    return;
                var body = ea.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);
                tcs.TrySetResult(response);
            };

            channel.BasicConsume(consumer: consumer,
                                 queue: QUEUE_NAME,
                                 autoAck: true);
        }

        public Task<string> CallAsync(string message, CancellationToken cancellationToken = default)
        {
            IBasicProperties props = channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = QUEUE_NAME;
            var messageBytes = Encoding.UTF8.GetBytes(message);
            // var decodedMessage = JsonSerializer.Deserialize<Request<UserAccount>>(messageBytes);
            var tcs = new TaskCompletionSource<string>();
            callbackMapper.TryAdd(correlationId, tcs);

            channel.BasicPublish(exchange: string.Empty,
                                 routingKey: QUEUE_NAME,
                                 basicProperties: props,
                                 body: messageBytes);

            cancellationToken.Register(() => callbackMapper.TryRemove(correlationId, out _));
            return tcs.Task;
        }

        public void Dispose()
        {
            connection.Close();
        }
    }
}

using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQConsumer;

namespace RabbitMQConnsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            const string queueName = "chat-demo";

            var factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@localhost:5672") };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            Console.WriteLine("Chat window:");

            channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var deserialize = JsonConvert.DeserializeObject<Message>(json);
                var result = new Message
                {
                    Name = deserialize.Name,
                    Content = deserialize.Content,
                };

                Console.WriteLine($"{result.Name} - {result.Content}");
            };

            channel.BasicConsume(queueName, true, consumer);
            Console.ReadLine();
        }
    }
}

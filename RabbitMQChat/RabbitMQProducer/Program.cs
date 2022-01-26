using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace RabbitMQProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            const string queueName = "chat-demo";

            var factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@localhost:5672") };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            Console.WriteLine("Message window:");

            Console.Write("Enter username: ");
            var username = Console.ReadLine();
            Console.Write("Enter Text: ");
            channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            while (connection.IsOpen)
            {
                var text = Console.ReadLine();

                var message = new { Name = username, Content = text };
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                channel.BasicPublish("", queueName, null, body);
            }
        }
    }
}

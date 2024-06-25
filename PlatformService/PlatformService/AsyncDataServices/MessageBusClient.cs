using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration p_configuration;
        private readonly IConnection p_connection;
        private readonly IModel p_channel;

        public MessageBusClient(IConfiguration _configuration)
        {
            p_configuration = _configuration;
            var factory = new ConnectionFactory() { HostName = p_configuration["RabbitMQHost"],
            Port = int.Parse(p_configuration["RabbitMQPort"])};
            try
            {
                p_connection = factory.CreateConnection();
                p_channel = p_connection.CreateModel();

                p_channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout, autoDelete: false);
                p_connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                System.Console.WriteLine("--> Connected to MessageBus");
            }
            catch(Exception ex)
            {
                System.Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
            }
        }
        public void PublishNewPlatform(PlatformPublishedDto _platformPublishedDto)
        {
            var message = JsonSerializer.Serialize(_platformPublishedDto);

            if(p_connection.IsOpen)
            {
                System.Console.WriteLine("--> RabbitMQ connection Open, sending message...");
                SendMessage(message);
            }
            else
            {
                System.Console.WriteLine("--> RabbitMQ connection closed, not sending");
            }
        }
        private void SendMessage(string _message)
        {
            var body = Encoding.UTF8.GetBytes(_message);

            p_channel.BasicPublish(exchange: "trigger",
                            routingKey: "",
                            basicProperties: null,
                            body: body);

            System.Console.WriteLine($"--> We have sent {_message}");
        }

        public void Dispose()
        {
            System.Console.WriteLine("MessageBus disposed");
            if(p_channel.IsOpen)
            {
                p_channel.Close();
                p_connection.Close();
            }
        }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            System.Console.WriteLine("--> RabbitMQ Connection Shutdown");
        }
    }
}
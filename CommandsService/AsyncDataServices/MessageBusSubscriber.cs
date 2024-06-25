
using System.Text;
using CommandsService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration p_configuration;
        private readonly IEventProcessor p_eventProcessor;

        private IConnection p_connection;
        private IModel p_channel;
        private string p_queueName;

        public MessageBusSubscriber(IConfiguration _configuration, IEventProcessor _eventProcessor)
        {
            p_configuration = _configuration;
            p_eventProcessor = _eventProcessor;

            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory() { HostName = p_configuration["RabbitMQHost"], Port = int.Parse(p_configuration["RabbitMQPort"]) };

            p_connection = factory.CreateConnection();
            p_channel = p_connection.CreateModel();
            p_channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            p_queueName = p_channel.QueueDeclare().QueueName;
            p_channel.QueueBind(queue: p_queueName,
                exchange: "trigger",
                routingKey: "");

            System.Console.WriteLine("--> Listening on the Message Bus...");

            p_connection.ConnectionShutdown += RabbitMq_ConnectionShutdown;
        }
        protected override Task ExecuteAsync(CancellationToken _stoppingToken)
        {
            _stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(p_channel);

            consumer.Received += (ModuleHandle, ea) =>
            {
                System.Console.WriteLine("--> Event Received!");

                var body = ea.Body;

                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                p_eventProcessor.ProcessEvent(notificationMessage);
            };

            p_channel.BasicConsume(queue: p_queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        private void RabbitMq_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            System.Console.WriteLine("--> Connection Shutdown");
        }

        public override void Dispose()
        {
            if(p_channel.IsOpen)
            {
                p_channel.Close();
                p_connection.Close();
            }

            base.Dispose();
        }
    }
}
using System.Text;
using System.Threading.Channels;
using CommandService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.AsyncDataServices;

public class MessageBusSubscriber : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IEventProcessor _eventProcessor;
    private IConnection _connection;
    private IModel _channel;
    private IModel _channel2;
    private string _queueName;
    private string _queueName2;

    public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
    {
        _configuration = configuration;
        _eventProcessor = eventProcessor;

        InitializeRabbitMQ();
    }

    private void InitializeRabbitMQ()
    {
        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMQHost"],
            Port = int.Parse(_configuration["RabbitMQPort"])
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
        _queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queue: _queueName,
            exchange: "trigger",
            routingKey: string.Empty);

        _channel2 = _connection.CreateModel();
        _channel2.ExchangeDeclare(exchange: "kiss", type: ExchangeType.Topic);
        _queueName2 = _channel2.QueueDeclare().QueueName;
        _channel2.QueueBind(queue: _queueName2,
            exchange: "kiss",
            routingKey: "sexy");

        Console.WriteLine(">> Listening on the Message Bus...");

        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
    }


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (ModuleHandle, ea) =>
        {
            Console.WriteLine(">> Event Received!");

            var body = ea.Body;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

            _eventProcessor.ProcessEvent(notificationMessage);
        };

        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);


        var consumer2 = new EventingBasicConsumer(_channel2);

        consumer2.Received += (ModuleHandle, ea) =>
        {
            

            var body = ea.Body;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
            Console.WriteLine($">> Event kiss Received!  notificationMessage is {notificationMessage}");
            //_eventProcessor.ProcessEvent(notificationMessage);
        };

        _channel2.BasicConsume(queue: _queueName2, autoAck: true, consumer: consumer2);

        return Task.CompletedTask;
    }

    private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        Console.WriteLine($">> RabbitMQ Connection Shutdown. {e.Cause}");
    }


    public void Dispose()
    {
        Console.WriteLine(">> MessageBus Disposed");
        if (_channel.IsOpen)
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
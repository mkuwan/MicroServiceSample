using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices;

public class MessageBusClient: IMessageBusClient
{
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IModel _channel2;

    public MessageBusClient(IConfiguration configuration)
    {
        _configuration = configuration;

        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMQHost"],
            Port = int.Parse(_configuration["RabbitMQPort"])
        };

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

            _channel2 = _connection.CreateModel();
            _channel2.ExchangeDeclare(exchange: "kiss", type: ExchangeType.Topic);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

            Console.WriteLine(">> Connected to RabbitMQ MessageBus");
        }
        catch (Exception ex)
        {
            Console.WriteLine($">> Could not connect to the RabbitMQ Message Bus: {ex.Message}");
        }
    }


    public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
    {
        var message = JsonSerializer.Serialize(platformPublishedDto);

        if (_connection.IsOpen)
        {
            Console.WriteLine(">> RabbitMQ Connection Open, sending message...");

            // send the message
            SendMessage(message);
        }
        else
        {
            Console.WriteLine(">> RabbitMQ connection is closed, not sending...");
        }
    }


    private void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        //for (int i = 1; i < 10001; i++)
        //{
        //    _channel.BasicPublish(exchange: "trigger",
        //        routingKey: string.Empty,
        //        basicProperties: null,
        //        body: body);
        //    Console.WriteLine($"RabbitMq BasicPublish {i}回目");
        //}

        //for (int i = 0; i < 100001; i++)
        //{
        //    _channel2.BasicPublish(exchange: "kiss",
        //        routingKey: "sexy",
        //        basicProperties: null,
        //        body: body);
        //    Console.WriteLine($"RabbitMq BasicPublish kiss {i}回目");
        //}

        _channel.BasicPublish(exchange: "trigger",
            routingKey: string.Empty,
            basicProperties: null,
            body: body);

        _channel2.BasicPublish(exchange: "kiss",
            routingKey: "sexy",
            basicProperties: null,
            body: body);

        Console.WriteLine($">> We have sent {message}");
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


    private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        Console.WriteLine($">> RabbitMQ Connection Shutdown. {e.Cause}");
    }
}
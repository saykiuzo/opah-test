using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using System.Text;

namespace FluxoCaixa.Shared.Messaging;

public interface IMessageBus
{
    void Publish<T>(T message, string queueName) where T : class;
    void Subscribe<T>(string queueName, Func<T, Task> handler) where T : class;
}

public class RabbitMqMessageBus : IMessageBus, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqMessageBus(string connectionString)
    {
        Console.WriteLine($"[RabbitMQ] Conectando usando: {connectionString}");
        var factory = new ConnectionFactory()
        {
            Uri = new Uri(connectionString)
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        Console.WriteLine($"[RabbitMQ] Conexão estabelecida com sucesso!");
    }

    public void Publish<T>(T message, string queueName) where T : class
    {
        Console.WriteLine($"[RabbitMQ] Declarando queue: {queueName}");
        _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var json = JsonConvert.SerializeObject(message);
        Console.WriteLine($"[RabbitMQ] Serializado JSON: {json}");
        var body = Encoding.UTF8.GetBytes(json);

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        Console.WriteLine($"[RabbitMQ] Publicando na queue: {queueName}");
        _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: body);
        Console.WriteLine($"[RabbitMQ] Mensagem publicada com sucesso!");
    }

    public void Subscribe<T>(string queueName, Func<T, Task> handler) where T : class
    {
        Console.WriteLine($"[RabbitMQ] Subscribe - Declarando queue: {queueName}");
        _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            Console.WriteLine($"[RabbitMQ] MENSAGEM RECEBIDA na queue: {queueName}!");
            Task.Run(async () =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"[RabbitMQ] JSON recebido: {json}");
                    var message = JsonConvert.DeserializeObject<T>(json);

                    if (message != null)
                    {
                        Console.WriteLine($"[RabbitMQ] Deserialização OK, chamando handler...");
                        await handler(message);
                        _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        Console.WriteLine($"[RabbitMQ] Handler executado com sucesso!");
                    }
                    else
                    {
                        Console.WriteLine($"[RabbitMQ] ERRO: Deserialização retornou null!");
                        _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RabbitMQ] EXCEÇÃO CAPTURADA: {ex.GetType().Name}: {ex.Message}");
                    Console.WriteLine($"[RabbitMQ] Stack trace: {ex.StackTrace}");
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            });
        };

        Console.WriteLine($"[RabbitMQ] Iniciando consumer na queue: {queueName}");
        _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        Console.WriteLine($"[RabbitMQ] Consumer ativo na queue: {queueName}");
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
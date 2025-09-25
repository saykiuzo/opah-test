using System.Collections.Concurrent;

namespace FluxoCaixa.Shared.Messaging;

public class InMemoryMessageBus : IMessageBus
{
    private readonly ConcurrentDictionary<string, List<object>> _handlers = new();
    private readonly object _lock = new();

    public void Publish<T>(T message, string queueName) where T : class
    {
        if (_handlers.TryGetValue(queueName, out var handlerList))
        {
            Task.Run(async () =>
            {
                foreach (var handler in handlerList.ToList())
                {
                    if (handler is Func<T, Task> typedHandler)
                    {
                        try
                        {
                            await typedHandler(message);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Erro ao processar mensagem: {ex.Message}");
                        }
                    }
                }
            });
        }
    }

    public void Subscribe<T>(string queueName, Func<T, Task> handler) where T : class
    {
        lock (_lock)
        {
            if (!_handlers.ContainsKey(queueName))
            {
                _handlers[queueName] = new List<object>();
            }
            _handlers[queueName].Add(handler);
        }
    }
}
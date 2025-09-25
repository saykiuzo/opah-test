using FluxoCaixa.Shared.Events;
using FluxoCaixa.Shared.Messaging;
using Consolidado.Application.EventHandlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Consolidado.API.Services;

public class MessageConsumerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<MessageConsumerService> _logger;

    public MessageConsumerService(
        IServiceProvider serviceProvider,
        IMessageBus messageBus,
        ILogger<MessageConsumerService> logger)
    {
        _serviceProvider = serviceProvider;
        _messageBus = messageBus;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Message Consumer Service iniciado");
        Console.WriteLine("[MessageConsumerService] Serviço iniciado - configurando subscriber");

        _messageBus.Subscribe<LancamentoCriadoEvent>("lancamento-criado", async (evento) =>
        {
            Console.WriteLine($"[MessageConsumerService] EVENTO RECEBIDO! ID: {evento.Id}, Data: {evento.Data}, Valor: {evento.Valor}");
            
            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<LancamentoCriadoEventHandler>();
            
            try
            {
                Console.WriteLine($"[MessageConsumerService] Processando evento com handler...");
                await handler.Handle(evento);
                _logger.LogInformation("Evento processado com sucesso: {EventoId}", evento.Id);
                Console.WriteLine($"[MessageConsumerService] Evento processado com SUCESSO!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar evento: {EventoId}", evento.Id);
                Console.WriteLine($"[MessageConsumerService] ERRO ao processar evento: {ex.Message}");
                throw; // Re-throw para que o RabbitMQ faça requeue
            }
        });

        Console.WriteLine("[MessageConsumerService] Subscriber configurado - aguardando mensagens...");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Message Consumer Service parando");
        await base.StopAsync(cancellationToken);
    }
}
using FluxoCaixa.Shared.Events;
using Consolidado.Domain.Entities;
using Consolidado.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Consolidado.Application.EventHandlers;

public class LancamentoCriadoEventHandler
{
    private readonly IConsolidadoDiarioRepository _repository;
    private readonly ILogger<LancamentoCriadoEventHandler> _logger;

    public LancamentoCriadoEventHandler(
        IConsolidadoDiarioRepository repository,
        ILogger<LancamentoCriadoEventHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Handle(LancamentoCriadoEvent evento)
    {
        const int maxRetries = 3;
        for (int tentativa = 1; tentativa <= maxRetries; tentativa++)
        {
            try
            {
                _logger.LogInformation("Processando lançamento criado. ID: {Id}, Data: {Data}, Tentativa: {Tentativa}", evento.Id, evento.Data, tentativa);
                Console.WriteLine($"[LancamentoCriadoEventHandler] Processando evento ID: {evento.Id} - Tentativa {tentativa}");

                var consolidadoExistente = await _repository.ObterPorDataAsync(evento.Data);

                if (consolidadoExistente == null)
                {
                    Console.WriteLine($"[LancamentoCriadoEventHandler] Consolidado não existe para {evento.Data}, criando novo");
                    var ultimoConsolidado = await _repository.ObterUltimoConsolidadoAsync();
                    var saldoInicial = ultimoConsolidado?.SaldoFinal ?? 0;

                    var novoConsolidado = new ConsolidadoDiario(evento.Data, saldoInicial);
                    
                    Console.WriteLine($"[LancamentoCriadoEventHandler] Adicionando {evento.Tipo} de valor {evento.Valor} ao novo consolidado");
                    if (evento.Tipo == TipoLancamento.Debito)
                    {
                        novoConsolidado.AdicionarDebito(evento.Valor);
                    }
                    else
                    {
                        novoConsolidado.AdicionarCredito(evento.Valor);
                    }

                    await _repository.AdicionarAsync(novoConsolidado);
                }
                else
                {
                    Console.WriteLine($"[LancamentoCriadoEventHandler] Atualizando consolidado existente para {evento.Data}");
                    Console.WriteLine($"[LancamentoCriadoEventHandler] Adicionando {evento.Tipo} de valor {evento.Valor} ao consolidado existente");
                    if (evento.Tipo == TipoLancamento.Debito)
                    {
                        consolidadoExistente.AdicionarDebito(evento.Valor);
                    }
                    else
                    {
                        consolidadoExistente.AdicionarCredito(evento.Valor);
                    }

                    await _repository.AtualizarAsync(consolidadoExistente);
                }

                await _repository.SaveChangesAsync();
                
                Console.WriteLine($"[LancamentoCriadoEventHandler] Consolidado salvo com sucesso na tentativa {tentativa}!");
                _logger.LogInformation("Consolidado atualizado com sucesso para a data {Data}", evento.Data);
                return; // Sucesso - sair do loop
            }
            catch (Exception ex) when (tentativa < maxRetries && 
                (ex.Message.Contains("expected to affect 1 row(s), but actually affected 0 row(s)") ||
                 ex.Message.Contains("concurrency")))
            {
                Console.WriteLine($"[LancamentoCriadoEventHandler] Erro de concorrência na tentativa {tentativa}: {ex.Message}");
                Console.WriteLine($"[LancamentoCriadoEventHandler] Aguardando {tentativa * 100}ms antes da próxima tentativa...");
                await Task.Delay(tentativa * 100); // Backoff progressivo
                continue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar lançamento criado. ID: {Id}, Tentativa: {Tentativa}", evento.Id, tentativa);
                Console.WriteLine($"[LancamentoCriadoEventHandler] ERRO na tentativa {tentativa}: {ex.Message}");
                throw; // Re-throw para que o RabbitMQ possa fazer requeue
            }
        }

        var finalError = $"Falhou ao processar evento após {maxRetries} tentativas devido a conflitos de concorrência";
        _logger.LogError(finalError + ". ID: {Id}", evento.Id);
        Console.WriteLine($"[LancamentoCriadoEventHandler] {finalError}");
        throw new InvalidOperationException(finalError);
    }
}
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Lancamentos.Application.Commands;
using Lancamentos.Application.Queries;
using Lancamentos.Application.DTOs;
using Lancamentos.Domain.Entities;

namespace Lancamentos.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LancamentosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<LancamentosController> _logger;

    public LancamentosController(IMediator mediator, ILogger<LancamentosController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(LancamentoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LancamentoResponse>> CriarLancamento([FromBody] CriarLancamentoRequest request)
    {
        try
        {
            var command = new CriarLancamentoCommand
            {
                Data = request.Data,
                Valor = request.Valor,
                Tipo = request.Tipo,
                Descricao = request.Descricao
            };

            var resultado = await _mediator.Send(command);
            
            _logger.LogInformation("Lançamento criado com sucesso. ID: {Id}", resultado.Id);
            
            return CreatedAtAction(nameof(ObterLancamentoPorId), new { id = resultado.Id }, resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar lançamento");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LancamentoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LancamentoResponse>>> ObterLancamentos(
        [FromQuery] DateTime? dataInicio = null,
        [FromQuery] DateTime? dataFim = null)
    {
        var query = new ObterLancamentosQuery
        {
            DataInicio = dataInicio,
            DataFim = dataFim
        };

        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LancamentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LancamentoResponse>> ObterLancamentoPorId(Guid id)
    {
        var query = new ObterLancamentoPorIdQuery { Id = id };
        var resultado = await _mediator.Send(query);

        if (resultado == null)
        {
            return NotFound(new { message = "Lançamento não encontrado" });
        }

        return Ok(resultado);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(LancamentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LancamentoResponse>> AtualizarLancamento(Guid id, [FromBody] AtualizarLancamentoRequest request)
    {
        try
        {
            var command = new AtualizarLancamentoCommand
            {
                Id = id,
                Data = request.Data,
                Valor = request.Valor,
                Tipo = (TipoLancamento)request.Tipo,
                Descricao = request.Descricao
            };

            var resultado = await _mediator.Send(command);
            
            _logger.LogInformation("Lançamento atualizado com sucesso. ID: {Id}", id);
            
            return Ok(resultado);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Lançamento não encontrado. ID: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar lançamento. ID: {Id}", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> RemoverLancamento(Guid id)
    {
        try
        {
            var command = new RemoverLancamentoCommand { Id = id };
            var resultado = await _mediator.Send(command);

            if (!resultado)
            {
                _logger.LogWarning("Tentativa de remover lançamento não encontrado. ID: {Id}", id);
                return NotFound(new { message = $"Lançamento com ID {id} não encontrado" });
            }

            _logger.LogInformation("Lançamento removido com sucesso. ID: {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover lançamento. ID: {Id}", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult HealthCheck()
    {
        return Ok(new { 
            status = "healthy", 
            timestamp = DateTime.UtcNow,
            service = "Lancamentos.API"
        });
    }
}